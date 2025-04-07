using System.Collections.Generic;

[SingletonMode(true)]
public class TargetManager : Singleton<TargetManager>
{
    public static readonly List<Target> AllTargets = new List<Target>();
    public static int AllTargetsCount { get; private set; }

    private static readonly Dictionary<Team, List<Target>> _targetsDictionary = new();
    private static readonly Dictionary<Team, int> _targetCountDictionary = new();
    private static readonly Dictionary<Team, Target> _targetLeaderDictionary = new();
    private static bool _dictionariesInvalid = true;

    public static void AddCardTarget(Target cardTarget)
    {
        // Invalidate dictionary
        _dictionariesInvalid = true;

        AllTargets.Add(cardTarget);
        AllTargetsCount++;
    }

    public static bool RemoveCardTarget(Target cardTarget)
    {
        // Invalidate dictionary
        _dictionariesInvalid = true;

        bool success = AllTargets.Remove(cardTarget);

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
}
