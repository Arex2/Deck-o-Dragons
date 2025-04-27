using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Video;
using Random = UnityEngine.Random;

/// <summary>
/// The main card class that every single card uses. <para/>
/// Does nothing on it's own and needs a <see cref="CardComponent"/> (or multiple) to work.
/// </summary>
// Script by Ruben
[CreateAssetMenu(menuName = "Cards/Create New Card")]
public class Card : GUIDScriptableObject
{
    public CardComponent CurrentComponent { get; private set; }

    public bool WaitingForCardsToAffect { get; private set; }

    public List<CardObject> CardsToAffect { get; private set; } = new();

    private static readonly Regex _descriptionKeywordRegex = new Regex(@"\{[\w\s\:]+\}", RegexOptions.IgnoreCase);

    public Sprite Sprite => sprite;

    public string DisplayName
    {
        get
        {
            if (string.IsNullOrEmpty(_cachedDisplayName))
            {
                _cachedDisplayName = string.IsNullOrEmpty(displayName) ? name : displayName;
            }

            return _cachedDisplayName;
        }
    }
    private string _cachedDisplayName;

    public int Cost => cost;
    public Element Element => element;
    public CardCategory Category => category;
    public CardRarity Rarity => rarity;

    /// <summary>
    /// The current level of the card. <para/>
    /// Negative numbers are for downgrades and positive numbers are for upgrades. <para/>
    /// 0 means no upgrade or downgrades and is the base level of the card.
    /// </summary>
    public int Level
    {
        get => _level;
    }
    private int _level = 0;

    public bool CanChangeLevel => canChangeLevel;
    public Optional<int> MinLevel => minLevel;
    public Optional<int> MaxLevel => maxLevel;

    public CardTag[] Tags => tags;

    /// <summary>
    /// The current <see cref="Target"/> that's using this <see cref="Card"/>.
    /// </summary>
    public Target User { get; private set; }

    public int Copies => copies;

    private CardData _cardData;

    [SerializeField] private Sprite sprite;

    [Space]
    [SerializeField] private string displayName;
    [SerializeField] private string description;

    [Space]
    [SerializeField] private int cost;
    [SerializeField] private Element element;
    [SerializeField] private CardCategory category;
    [SerializeField] private CardRarity rarity;
    [SerializeField] private int copies = 1;

    [SerializeField] private bool canChangeLevel = false;
    [SerializeField] private Optional<int> minLevel;
    [SerializeField] private Optional<int> maxLevel;

    [Space]
    [SerializeField] private CardTag[] tags;
    private HashSet<CardTag> _tagsHashSet = null;

    [HideInInspector]
    [SerializeField] private CardComponent[] cardComponents;
    private Dictionary<Type, CardComponent[]> _cardComponentTypeDictionary = new();
    private Dictionary<string, CardComponent> _cardComponentNameDictionary = new();

    public void OnLoad()
    {
        _cardComponentTypeDictionary.Clear();
        _cardComponentNameDictionary.Clear();

        // Forgive me for doing this... Whatever this is...
        Dictionary<Type, List<CardComponent>> temp = new();

        foreach (CardComponent cardComponent in cardComponents)
        {
            void AddType(Type type)
            {
                if (!temp.ContainsKey(type))
                {
                    temp.Add(type, new());
                }

                temp[type].Add(cardComponent);
            }

            void AddInterfaces(Type type)
            {
                foreach (Type interfaceType in type.GetInterfaces())
                {
                    AddType(interfaceType);
                }
            }

            Type type = cardComponent.GetType();

            AddType(type);
            AddInterfaces(type);

            while (type.BaseType != null && type.BaseType != typeof(ScriptableObject))
            {
                type = type.BaseType;

                AddType(type);
                AddInterfaces(type);
            }

            string name = cardComponent.name.Trim().ToLower();

            if (!_cardComponentNameDictionary.ContainsKey(name))
            {
                _cardComponentNameDictionary.Add(name, cardComponent);
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogWarning($"The Card: \"{this.name}\" has more than one CardComponent named \"{name}\"! Please rename them in the inspector.", this);
            }
#endif
        }

        foreach (var pair in temp)
        {
            _cardComponentTypeDictionary.Add(pair.Key, pair.Value.ToArray());
        }

        foreach (CardComponent cardComponent in cardComponents)
        {
            cardComponent.InternalInitialize();
        }

        _cachedDisplayName = null;
        //_descriptionCache = null;
        //Debug.Log("Description print test for: " + DisplayName + " = " + Description, this);
    }

    /// <summary>
    /// Returns whether or not this card has the given card <paramref name="tag"/>.
    /// </summary>
    public bool HasTag(CardTag tag)
    {
        if (_tagsHashSet == null)
        {
            _tagsHashSet = new(tags);
        }

        return _tagsHashSet.Contains(tag);
    }

    public IEnumerator Play(Target user, Action onFinish = null) => Play(user, new(), 0, onFinish);

    public IEnumerator Play(Target user, int level, Action onFinish = null) => Play(user, new(), level, onFinish);

    public IEnumerator Play(Target user, CardData cardData, Action onFinish = null) => Play(user, cardData, 0, onFinish);

    public IEnumerator Play(Target user, CardData cardData, int level, Action onFinish = null)
    {
        _cardData = cardData;

        foreach (CardComponent cardComponent in cardComponents)
        {
            cardComponent.OnBeforePlayed();
        }

        return PlayCoroutine(user, level, onFinish);
    }

    /// <summary>
    /// Will play this card with the <see cref="Target"/> that's playing the card being the given <paramref name="user"/>. <para/>
    /// <paramref name="onFinish"/> is invoked when this card has finished playing.
    /// </summary>
    private IEnumerator PlayCoroutine(Target user, int level, Action onFinish = null)
    {
        User = user;

        _level = level;

        foreach (CardComponent cardComponent in cardComponents)
        {
            if (!cardComponent.Enabled)
            {
                continue;
            }

            CurrentComponent = cardComponent;

            IAffectOtherCards affectOtherCards = cardComponent as IAffectOtherCards;

            if (affectOtherCards != null)
            {
                CardsToAffect.Clear();
                WaitingForCardsToAffect = true;

                yield return new WaitUntil(() => !WaitingForCardsToAffect);

                IEnumerator enumerator = affectOtherCards.OnCardsSelected(CardsToAffect);

                if (enumerator != null)
                {
                    yield return enumerator;
                }

                CardsToAffect.Clear();
            }

            TargetFilter targetFilter = cardComponent.TargetFilter;

            List<Target> targets = null;

            if (targetFilter != null)
            {
                IEnumerator enumerator = TargetManager.GetTargetsWithFilterCoroutine(user.Team, targetFilter, true);

                while (enumerator.MoveNext())
                {
                    object current = enumerator.Current;

                    if (current is List<Target>)
                    {
                        targets = (List<Target>)current;
                    }
                    else
                    {
                        yield return current;
                    }
                }
            }

            IUse use = cardComponent as IUse;
            IUseCoroutine useCoroutine = cardComponent as IUseCoroutine;
            IUseMulti useMulti = cardComponent as IUseMulti;
            IUseSingle useSingle = cardComponent as IUseSingle;
            IUseCoroutineMulti useCoroutineMulti = cardComponent as IUseCoroutineMulti;
            IUseCoroutineSingle useCoroutineSingle = cardComponent as IUseCoroutineSingle;

            if (use != null)
            {
                use.Use();
            }

            if (useCoroutine != null)
            {
                IEnumerator enumerator = useCoroutine.UseCoroutine();

                if (enumerator != null)
                {
                    yield return enumerator;
                }
            }

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

        CurrentComponent = null;

        foreach (CardComponent cardComponent in cardComponents)
        {
            cardComponent.OnAfterPlayed();
        }

        onFinish?.Invoke();

        User = null;
    }

    public void FinishedSettingCardsToAffect()
    {
        if (!WaitingForCardsToAffect)
        {
            return;
        }

        WaitingForCardsToAffect = false;
    }

    #region Card Data Stuff
    public void SetCardData<T>(string key, T value) => _cardData.SetCardData(key, value);

    public T GetCardData<T>(string key) => _cardData.GetCardData<T>(key);

    public T GetCardData<T>(string key, T defaultValue) => _cardData.GetCardData(key, defaultValue);

    public bool HasCardData<T>(string key) => _cardData.HasCardData<T>(key);

    public bool TryGetCardData<T>(string key, out T value) => _cardData.TryGetCardData(key, out value);

    public bool TryGetCardData<T>(string key, out T value, T defaultValue) => _cardData.TryGetCardData(key, out value, defaultValue);
    #endregion

    #region Description Stuff
    /// <summary>
    /// Returns a description for this card that's modified to match the given <paramref name="level"/>.
    /// </summary>
    public string GetDescription(int level = 0, CardData data = new())
    {
        _level = level;
        _cardData = data;

        //_descriptionCache = 
        return
            _descriptionKeywordRegex.IsMatch(description) ? 
            _descriptionKeywordRegex.Replace(description, DescriptionKeywordEvaluator)
            :
            description;
    }

    private string DescriptionKeywordEvaluator(Match match)
    {
        if (match.Success)
        {
            string keyword = match.Value.Substring(1, match.Value.Length - 2).Trim().ToLower();

            if (keyword.Contains(':'))
            {
                string[] split = keyword.Split(':');
                string name = split[0].Trim();
                string tempKeyword = split[1].Trim();

                if (TryGetCardComponent(name, out CardComponent cardComponent))
                {
                    string result = cardComponent.ReplaceDescriptionKeyword(tempKeyword);

                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            foreach (CardComponent cardComponent in cardComponents)
            {
                if (!cardComponent.ShouldReplaceDescriptionKeywords())
                {
                    continue;
                }

                string result = cardComponent.ReplaceDescriptionKeyword(keyword);

                if (result == null)
                {
                    continue;
                }

                return result;
            }

        }

        return match.Value;
    }
    #endregion

    #region GetCardComponent Methods
    public T GetCardComponent<T>() where T : CardComponent
    {
        return GetCardComponents<T>()[0];
    }

    public CardComponent GetCardComponent(Type type)
    {
        return GetCardComponents(type)[0];
    }

    public CardComponent GetCardComponent(string name, bool formatName = false)
    {
        if (TryGetCardComponent(name, out CardComponent result, formatName))
        {
            return result;
        }

        return null;
    }

    public T[] GetCardComponents<T>() where T : CardComponent
    {
        return GetCardComponents(typeof(T)) as T[];
    }

    public CardComponent[] GetCardComponents(Type type)
    {
        return _cardComponentTypeDictionary[type];
    }

    public bool TryGetCardComponent<T>(out T cardComponent) where T : CardComponent
    {
        return TryGetCardComponent(out cardComponent);
    }

    public bool TryGetCardComponent(string name, out CardComponent cardComponent, bool formatName = false)
    {
        if (formatName)
        {
            name = name.ToLower().Trim();
        }

        return _cardComponentNameDictionary.TryGetValue(name, out cardComponent);
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
        return _cardComponentTypeDictionary.TryGetValue(type, out cardComponents);
    }

    public bool HasCardComponent<T>()
    {
        return HasCardComponent(typeof(T));
    }

    public bool HasCardComponent(Type type)
    {
        return _cardComponentTypeDictionary.ContainsKey(type);
    }

    public bool HasCardComponent(string name, bool formatName = false)
    {
        if (formatName)
        {
            name = name.ToLower().Trim();
        }

        return _cardComponentNameDictionary.ContainsKey(name);
    }
    #endregion
}