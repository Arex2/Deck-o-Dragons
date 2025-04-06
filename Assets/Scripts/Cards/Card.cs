using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// The main card class that every single card uses. <para/>
/// Does nothing on it's own and needs a <see cref="CardComponent"/> (or multiple) to work.
/// </summary>
// Script by Ruben
[CreateAssetMenu(menuName = "Cards/Create New Card")]
public class Card : ScriptableObject
{
    public static bool PlayingACard { get; private set; } = false;

    public string DisplayName => displayName;
    public string Description => description;

    // TODO: Make this modifiable by card components
    public int Cost => cost;
    public Element Element => element;
    public CardCategory Category => category;

    // TODO: Upgrades
    public int Tier { get; private set; } = 0;

    [SerializeField] private string displayName;
    [SerializeField] private string description;

    [Space]
    [SerializeField] private int cost;
    [SerializeField] private Element element;
    [SerializeField] private CardCategory category;

    [Space]
    [SerializeField] private List<CardTag> tags = new();

    [HideInInspector]
    [SerializeField] private CardComponent[] cardComponents;
    private Dictionary<Type, CardComponent[]> _cardComponentDictionary = new();

    private Coroutine _coroutine;

    public void OnLoad()
    {
        Dictionary<Type, List<CardComponent>> temp = new();

        foreach (CardComponent cardComponent in cardComponents)
        {
            Type type = cardComponent.GetType();

            if (!HasCardComponent(type))
            {
                temp.Add(type, new());
            }

            temp[type].Add(cardComponent);

            cardComponent.Initialize();
        }

        _cardComponentDictionary = new();

        foreach (var pair in temp)
        {
            _cardComponentDictionary.Add(pair.Key, pair.Value.ToArray());
        }
    }

    public void Play(Target user, Action onFinish = null)
    {
        if (PlayingACard)
        {
            return;
        }

        _coroutine = CardManager.StartStaticCoroutine(PlayCoroutine(user, onFinish));

        PlayingACard = true;
    }

    private IEnumerator PlayCoroutine(Target user, Action onFinish = null)
    {
        Team ownTeam = user.Team;
        Team opponentTeam = ownTeam.GetOpponentTeam();

        Team GetRandomTeam() => Random.Range(0, 2) == 0 ? ownTeam : opponentTeam;

        List<Target> GetTargets(Team? team, out int count)
        {
            if (team.HasValue)
            {
                return TargetManager.GetTargets(team.Value, out count);
            }

            count = TargetManager.AllTargetsCount;
            return TargetManager.AllTargets;
        }

        List<Target> multiTargets = null;
        List<Target> singleTarget = new() { null };
        int count;
        bool doSingleTarget;

        foreach (CardComponent cardComponent in cardComponents)
        {
            TargetFilter targetFilter = cardComponent.TargetFilter;

            List<Target> targets = null;

            if (targetFilter != null)
            {
                Team? team;

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
                        Debug.Log("TODO!!! UI");
                        team = opponentTeam;
                        break;

                    // Chaos
                    case TargetFilter.FilterTeam.Random:
                        team = GetRandomTeam();
                        break;

                    // Default behaviour (also the behaviour if "TargetFilter.FilterTeam.All" is selected)
                    default:
                        team = null;
                        break;
                }

                switch (targetFilter.Mode)
                {
                    case TargetFilter.FilterMode.Leader:
                    case TargetFilter.FilterMode.Chosen:

                        // Failsafe
                        if (!team.HasValue)
                        {
                            // Select a random team
                            team = GetRandomTeam();
                        }

                        singleTarget[0] = TargetManager.GetLeader(team.Value);
                        doSingleTarget = true;
                        break;

                    /* TODO
                case TargetFilter.FilterMode.Chosen:
                    // TODO: Choose UI
                    singleTarget[0]
                    doSingleTarget = true;
                    break;
                    */

                    // Chaos
                    case TargetFilter.FilterMode.Random:
                        List<Target> list = GetTargets(team, out count);

                        singleTarget[0] = list[Random.Range(0, count)];
                        doSingleTarget = true;
                        break;

                    // Default behaviour (also the behaviour if "TargetFilter.FilterMode.All" is selected)
                    default:
                        doSingleTarget = false;
                        multiTargets = GetTargets(team, out count);
                        break;
                }

                targets = doSingleTarget ? singleTarget : multiTargets;
            }

            IUseMulti useMulti = cardComponent as IUseMulti;
            IUseSingle useSingle = cardComponent as IUseSingle;
            IUseCoroutineMulti useCoroutineMulti = cardComponent as IUseCoroutineMulti;
            IUseCoroutineSingle useCoroutineSingle = cardComponent as IUseCoroutineSingle;

            if (useMulti != null)
            {
                useMulti.Use(targets);
            }

            if (useSingle != null)
            {
                if (targets != null)
                {
                    foreach (Target target in targets)
                    {
                        useSingle.Use(target);
                    }
                }
                else
                {
                    useSingle.Use(null);
                }
            }

            if (useCoroutineMulti != null)
            {
                IEnumerator enumerator = useCoroutineMulti.UseCoroutine(targets);

                if (enumerator != null)
                {
                    yield return enumerator;
                }
            }

            if (useCoroutineSingle != null)
            {
                foreach (Target target in targets)
                {
                    IEnumerator enumerator = useCoroutineSingle.UseCoroutine(target);

                    if (enumerator != null)
                    {
                        yield return enumerator;
                    }
                }
            }
        }

        onFinish?.Invoke();

        PlayingACard = false;
    }

    #region GetCardComponent Methods
    public T GetCardComponent<T>() where T : CardComponent
    {
        return GetCardComponents<T>()[0];
    }

    public CardComponent GetCardComponent(Type type)
    {
        return GetCardComponents(type)[0];
    }

    public T[] GetCardComponents<T>() where T : CardComponent
    {
        return GetCardComponents(typeof(T)) as T[];
    }

    public CardComponent[] GetCardComponents(Type type)
    {
        return _cardComponentDictionary[type];
    }

    public bool TryGetCardComponent<T>(out T cardComponent) where T : CardComponent
    {
        return TryGetCardComponent(out cardComponent);
    }

    public bool TryGetCardComponent(Type type, out CardComponent cardComponent)
    {
        bool success = TryGetCardComponents(type, out CardComponent[] result);

        if (success)
        {
            cardComponent = result[0];
        }
        else
        {
            cardComponent = null;
        }

        return success;
    }

    public bool TryGetCardComponents<T>(out T[] cardComponents) where T : CardComponent
    {
        return TryGetCardComponents(out cardComponents);
    }

    public bool TryGetCardComponents(Type type, out CardComponent[] cardComponents)
    {
        return _cardComponentDictionary.TryGetValue(type, out cardComponents);
    }

    public bool HasCardComponent<T>()
    {
        return HasCardComponent(typeof(T));
    }

    public bool HasCardComponent(Type type)
    {
        return _cardComponentDictionary.ContainsKey(type);
    }
    #endregion
}