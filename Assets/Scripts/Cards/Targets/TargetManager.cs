using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[SingletonMode(true)]
public class TargetManager : Singleton<TargetManager>
{
    public static readonly List<Target> AllTargets = new List<Target>();
    public static int AllTargetsCount { get; private set; }

    private static readonly Dictionary<Team, List<Target>> _targetsDictionary = new();
    private static readonly Dictionary<Team, int> _targetCountDictionary = new();
    private static readonly Dictionary<Team, Target> _targetLeaderDictionary = new();
    private static bool _dictionariesInvalid = true;

    public static void AddTarget(Target target)
    {
        // Invalidate dictionary
        _dictionariesInvalid = true;

        AllTargets.Add(target);
        AllTargetsCount++;
    }

    public static bool RemoveTarget(Target target)
    {
        // Invalidate dictionary
        _dictionariesInvalid = true;

        bool success = AllTargets.Remove(target);

        if (success)
        {
            AllTargetsCount--;
        }

        return success;
    }

    public static List<Target> GetTargets(Team team, out int count)
    {
        // Create dictionaries if it's been invalidated
        if (_dictionariesInvalid)
        {
            CreateDictionaries();
        }

        count = _targetCountDictionary[team];

        return _targetsDictionary[team];
    }

    public static List<Target> GetTargets(Team team)
    {
        return GetTargets(team, out _);
    }

    public static Target GetLeader(Team team)
    {
        // Create dictionaries if it's been invalidated
        if (_dictionariesInvalid)
        {
            CreateDictionaries();
        }

        return _targetLeaderDictionary[team];
    }

    private static void CreateDictionaries()
    {
        _dictionariesInvalid = false;

        _targetsDictionary.Clear();
        _targetCountDictionary.Clear();
        _targetLeaderDictionary.Clear();

        foreach (Target target in AllTargets)
        {
            if (target.IsLeader)
            {
                _targetLeaderDictionary[target.Team] = target;
            }

            if (!_targetsDictionary.TryGetValue(target.Team, out List<Target> targets))
            {
                targets = new List<Target>();

                _targetsDictionary[target.Team] = targets;
                _targetCountDictionary[target.Team] = 0;
            }

            targets.Add(target);
            _targetCountDictionary[target.Team]++;
        }
    }

    public static List<Target> GetTargetsWithFilter(Team ownTeam, TargetFilter targetFilter)
    {
        IEnumerator enumerator = GetTargetsWithFilterCoroutine(ownTeam, targetFilter, false);

        while (enumerator.MoveNext())
        {
            object current = enumerator.Current;

            if (current is List<Target>)
            {
                return (List<Target>)current;
            }
        }

#if UNITY_EDITOR
        Debug.LogWarning("Something went wrong!");
#endif

        return null;
    }

    public static IEnumerator GetTargetsWithFilterCoroutine(Team ownTeam, TargetFilter targetFilter, bool useTargetSelector)
    {
        Team opponentTeam = ownTeam.GetOpponentTeam();

        Team? team;
        int count;

        List<Target> targets;

        void AssignSingleTarget(Target target)
        {
            targets = new() { target };
        }

        // Local method for getting all of the targets in a given team
        // Also gives an out value for the amount of targets
        // If no team is given (its nullable) then ALL targets will be used
        List<Target> GetTargets(Team? team, out int count)
        {
            if (team.HasValue)
            {
                return TargetManager.GetTargets(team.Value, out count);
            }

            count = AllTargetsCount;
            return AllTargets;
        }

        // Determine target
        switch (targetFilter.Team)
        {
            case TargetFilter.FilterTeam.Opponent:
                team = opponentTeam;
                break;

            case TargetFilter.FilterTeam.Own:
                team = ownTeam;
                break;

            case TargetFilter.FilterTeam.Chosen:
                if (useTargetSelector)
                {
                    yield return TargetSelector.SelectTeam();

                    team = TargetSelector.TeamResult;
                }
                // Target all
                else
                {
                    team = null;
                }
                break;

            // Chaos
            case TargetFilter.FilterTeam.Random:
                team = Teams.GetRandomTeam();
                break;

            // Default behaviour (also the behaviour if "TargetFilter.FilterTeam.All" is selected)
            default:
                team = null;
                break;
        }

        switch (targetFilter.Mode)
        {
            case TargetFilter.FilterMode.Leader:

                if (!team.HasValue)
                {
                    // Add all leaders
                    targets = new();

                    foreach (Team leaderTeam in Teams.AllTeams)
                    {
                        targets.Add(GetLeader(leaderTeam));
                    }
                    break;
                }

                AssignSingleTarget(GetLeader(team.Value));
                break;

            case TargetFilter.FilterMode.Chosen:
                List<Target> list = GetTargets(team, out count);

                if (count <= 1)
                {
                    AssignSingleTarget(list[0]);
                }
                else
                {
                    if (useTargetSelector)
                    {
                        yield return TargetSelector.SelectTarget(list, count);

                        AssignSingleTarget(TargetSelector.TargetResult);
                    }
                    // Target all
                    else
                    {
                        targets = list;
                    }
                }
                break;

            // Chaos
            case TargetFilter.FilterMode.Random:
                list = GetTargets(team, out count);

                AssignSingleTarget(list[Random.Range(0, count)]);
                break;

            // Default behaviour (also the behaviour if "TargetFilter.FilterMode.All" is selected)
            default:
                targets = GetTargets(team, out count);
                break;
        }

        yield return targets;
    }
}
