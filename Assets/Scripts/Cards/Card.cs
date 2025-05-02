using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
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
    [NonSerialized]
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

    public TagData Tags => _playTagData.HasValue ? _playTagData.Value : tags;

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
    [SerializeField] private TagData tags;
    [NonSerialized]
    private TagData? _playTagData = null; // Tag data used in the Play() method

    [HideInInspector]
    [SerializeField] private CardComponent[] cardComponents;
    private Dictionary<Type, CardComponent[]> _cardComponentTypeDictionary = new();
    private Dictionary<string, CardComponent> _cardComponentNameDictionary = new();

    [NonSerialized]
    private bool _cachedCardComponents = false;

    public void OnLoad()
    {
        TryCacheCardComponents();
    }

    public IEnumerator Play(Target user, Action onFinish = null) => Play(user, new(), null, 0, onFinish);

    public IEnumerator Play(Target user, int level, Action onFinish = null) => Play(user, new(), null, level, onFinish);

    public IEnumerator Play(Target user, CardData cardData, Action onFinish = null) => Play(user, cardData, null, 0, onFinish);

    public IEnumerator Play(Target user, CardData cardData, int level, Action onFinish = null) => Play(user, cardData, null, level, onFinish);

    public IEnumerator Play(Target user, CardData cardData, TagData? tags, int level, Action onFinish = null)
    {
        _cardData = cardData;
        _playTagData = tags;

        foreach (CardComponent cardComponent in cardComponents)
        {
            cardComponent.OnBeforeCardPlayed();
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

            // Account for and execute IAffectOtherCards
            IAffectOtherCardsHandler affectOtherCards = cardComponent as IAffectOtherCardsHandler;

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

            // Get targets using the target filter
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

            // Play the card component
            cardComponent.Play(targets);

            IEnumerator playCoroutine = cardComponent.PlayCoroutine(targets);

            if (playCoroutine != null)
            {
                yield return playCoroutine;
            }
        }

        CurrentComponent = null;

        foreach (CardComponent cardComponent in cardComponents)
        {
            cardComponent.OnAfterCardPlayed();
        }

        onFinish?.Invoke();

        User = null;
        _playTagData = null;
    }

    public void FinishedSettingCardsToAffect()
    {
        if (!WaitingForCardsToAffect)
        {
            return;
        }

        WaitingForCardsToAffect = false;
    }

    public void Cancel()
    {
        CurrentComponent = null;
        User = null;
        _playTagData = null;
    }

    /// <summary>
    /// Returns whether or not this card has the given card <paramref name="tag"/>.
    /// </summary>
    public bool HasTag(CardTag tag) => Tags.HasTag(tag);

    /// <summary>
    /// Returns the potency this <see cref="Card"/> has on the given <paramref name="tag"/>.
    /// </summary>
    public float GetTagPotency(CardTag tag) => Tags[tag];

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
    private static readonly MethodInfo _createListMethod = typeof(Card).GetMethod(nameof(CreateListMethod), BindingFlags.Static | BindingFlags.NonPublic);
    private static List<T> CreateListMethod<T>() => new();

    private static readonly MethodInfo _createArrayMethod = typeof(Card).GetMethod(nameof(CreateArrayMethod), BindingFlags.Static | BindingFlags.NonPublic);
    private static CardComponent[] CreateArrayMethod<T>(List<T> list) => list.ToArray() as CardComponent[];

    private void TryCacheCardComponents()
    {
        if (_cachedCardComponents)
        {
            return;
        }

        _cachedCardComponents = true;

        _cardComponentTypeDictionary.Clear();
        _cardComponentNameDictionary.Clear();

        // Forgive me for doing this... Whatever this is...
        Dictionary<Type, IList> temp = new();

        foreach (CardComponent cardComponent in cardComponents)
        {
            void AddType(Type type)
            {
                if (!temp.ContainsKey(type))
                {
                    temp.Add(type, _createListMethod.MakeGenericMethod(type).Invoke(this, null) as IList);
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
            Type type = pair.Key;
            CardComponent[] array = _createArrayMethod.MakeGenericMethod(type).Invoke(null, new object[] { pair.Value }) as CardComponent[];

            _cardComponentTypeDictionary.Add(type, array);
        }

        foreach (CardComponent cardComponent in cardComponents)
        {
            cardComponent.InternalInitialize();
        }
    }

    public T GetCardComponent<T>() where T : CardComponent
    {
        return GetCardComponent(typeof(T)) as T;
    }

    public CardComponent GetCardComponent(Type type)
    {
        CardComponent[] cardComponents = GetCardComponents(type);

        if (cardComponents == null)
        {
            return null;
        }

        return cardComponents[0];
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
        CardComponent[] cardComponents = GetCardComponents(typeof(T));

        if (cardComponents == null)
        {
            return null;
        }

        return cardComponents as T[];
    }

    public CardComponent[] GetCardComponents(Type type)
    {
        TryCacheCardComponents();

        if (type == null)
        {
            return null;
        }

        if (!_cardComponentTypeDictionary.ContainsKey(type))
        {
            return null;
        }

        return _cardComponentTypeDictionary[type];
    }

    public bool TryGetCardComponent(string name, out CardComponent cardComponent, bool formatName = false)
    {
        TryCacheCardComponents();

        if (formatName)
        {
            name = name.ToLower().Trim();
        }

        return _cardComponentNameDictionary.TryGetValue(name, out cardComponent);
    }

    public bool TryGetCardComponent<T>(out T cardComponent) where T : CardComponent
    {
        bool success = TryGetCardComponent(typeof(T), out CardComponent result);

        if (success)
        {
            cardComponent = result as T;
        }
        else
        {
            cardComponent = null;
        }

        return success;
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
        bool success = TryGetCardComponents(typeof(T), out CardComponent[] result);

        if (success)
        {
            cardComponents = result as T[];
        }
        else
        {
            cardComponents = null;
        }

        return success;
    }

    public bool TryGetCardComponents(Type type, out CardComponent[] cardComponents)
    {
        TryCacheCardComponents();

        return _cardComponentTypeDictionary.TryGetValue(type, out cardComponents);
    }

    public bool HasCardComponent<T>()
    {
        return HasCardComponent(typeof(T));
    }

    public bool HasCardComponent(Type type)
    {
        TryCacheCardComponents();

        return _cardComponentTypeDictionary.ContainsKey(type);
    }

    public bool HasCardComponent(string name, bool formatName = false)
    {
        TryCacheCardComponents();

        if (formatName)
        {
            name = name.ToLower().Trim();
        }

        return _cardComponentNameDictionary.ContainsKey(name);
    }
    #endregion
}