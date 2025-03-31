using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    #region Singleton Instance
    public static TargetManager Instance { get; private set; }

    [RuntimeInitializeOnLoadMethod]
    public static void Init()
    {
        new GameObject(nameof(TargetManager), typeof(TargetManager));
    }
    #endregion

    private static readonly List<Target> _allTargets = new List<Target>();

    private static Dictionary<Team, List<Target>> _targetsDictionary = null;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void AddCardTarget(Target cardTarget)
    {
        // Invalidate dictionary
        _targetsDictionary = null;

        _allTargets.Add(cardTarget);
    }

    public static bool RemoveCardTarget(Target cardTarget)
    {
        // Invalidate dictionary
        _targetsDictionary = null;

        return _allTargets.Remove(cardTarget);
    }

    public static List<Target> GetTargets(Team team)
    {
        // Create dictionary if it's been invalidated
        if (_targetsDictionary == null)
        {
            _targetsDictionary = new();

            foreach (Target target in _allTargets)
            {
                if (!_targetsDictionary.TryGetValue(target.Team, out List<Target> targets))
                {
                    targets = new List<Target>();

                    _targetsDictionary[target.Team] = targets;
                }

                targets.Add(target);
            }
        }

        return _targetsDictionary[team];
    }
}
