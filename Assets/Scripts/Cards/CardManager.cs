using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The script that has a reference to every <see cref="Card"/> and <see cref="CardTag"/>.
/// </summary>
// Script by Ruben
[SingletonMode(true)]
public class CardManager : Singleton<CardManager>
{
    public static Card[] AllCards => Instance.allCards;
    public static Card[] NonDrawableCards => Instance._nonDrawableCards;
    public static Card[] DrawableCards => Instance._drawableCards;
    public static CardTag[] AllTags => Instance.allTags;
    public static CardTag NonDrawableTag => Instance.nonDrawableTag;

    [HideInInspector] [SerializeField] private Card[] allCards;
    [HideInInspector] [SerializeField] private CardTag[] allTags;

    private Card[] _nonDrawableCards;
    private Card[] _drawableCards;

    [SerializeField] private CardTag nonDrawableTag;

    private static readonly Dictionary<CardTag, Card[]> _cardsTagDictionary = new();
    private static readonly Dictionary<string, Card> _cardsNameDictionary = new();
    private static readonly Dictionary<string, Card> _cardsGUIDDictionary = new();
    private static readonly Dictionary<string, CardTag> _cardTagsNameDictionary = new();
    private static readonly Dictionary<string, CardTag> _cardTagsGUIDDictionary = new();

    protected override void Awake()
    {
        base.Awake();

        foreach (CardTag tag in allTags)
        {
            if (tag == null)
            {
                continue;
            }

            string name = tag.name.ToLower().Trim();

            if (!_cardTagsNameDictionary.ContainsKey(name))
            {
                _cardTagsNameDictionary.Add(tag.name.ToLower().Trim(), tag);
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogWarning($"There are multiple CardTags with the name \"{name}\"! Please rename one of them.");
            }
#endif
            _cardTagsGUIDDictionary.Add(tag.GUID, tag);
        }

        Dictionary<CardTag, List<Card>> temp = new();
        List<Card> enemyCards = new();
        List<Card> nonEnemyCards = new();

        foreach (Card card in allCards)
        {
            if (card == null)
            {
                continue;
            }

            foreach (CardTag tag in card.Tags)
            {
                if (tag == null)
                {
                    continue;
                }

                if (!temp.ContainsKey(tag))
                {
                    temp.Add(tag, new());
                }

                temp[tag].Add(card);
            }

            string name = card.name.ToLower().Trim();

            if (!_cardsNameDictionary.ContainsKey(name))
            {
                _cardsNameDictionary.Add(name, card);
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogWarning($"There are multiple Cards with the name \"{name}\"! Please rename one of them.");
            }
#endif

            if (string.IsNullOrEmpty(card.GUID))
            {
#if UNITY_EDITOR
                Debug.LogWarning($"The card \"{card.name}\" is missing a GUID! The fix to this is to simply select the card in the inspector.", card);
#endif
                continue;
            }

            if (!_cardsGUIDDictionary.ContainsKey(card.GUID))
            {
                _cardsGUIDDictionary.Add(card.GUID, card);
            }
#if UNITY_EDITOR
            else
            {
                // Should be impossible
                Debug.LogWarning($"There are multiple Cards with the GUID \"{card.GUID}\"! How could this happen?");
            }
#endif

            if (card.HasTag(nonDrawableTag))
            {
                enemyCards.Add(card);
            }
            else
            {
                nonEnemyCards.Add(card);
            }
        }

        _nonDrawableCards = enemyCards.ToArray();
        _drawableCards = nonEnemyCards.ToArray();

        foreach (var pair in temp)
        {
            _cardsTagDictionary.Add(pair.Key, pair.Value.ToArray());
        }

        foreach (Card card in allCards)
        {
            if (card == null)
            {
                continue;
            }

            card.OnLoad();
        }
    }

    public static Coroutine StartStaticCoroutine(IEnumerator method)
    {
        return Instance.StartCoroutine(method);
    }

    public static void StopStaticCoroutine(Coroutine coroutine)
    {
        Instance.StopCoroutine(coroutine);
    }

    /// <summary>
    /// Returns an array of <see cref="Card"/>s that have the attached <paramref name="tag"/>.
    /// </summary>
    public static Card[] GetCardsByTag(CardTag tag)
    {
        if (_cardsTagDictionary.TryGetValue(tag, out Card[] cards))
        {
            return cards;
        }

        Debug.LogWarning($"There are no Cards with the tag: \"{tag.name}\"", tag);

        return null;
    }

    /// <summary>
    /// Returns a <see cref="Card"/> with the given <paramref name="name"/>.
    /// </summary>
    public static Card GetCardByName(string name)
    {
        if (_cardsNameDictionary.TryGetValue(name, out Card card))
        {
            return card;
        }

        Debug.LogWarning($"There is no Card with the name: \"{name}\"");

        return null;
    }

    /// <summary>
    /// Returns a <see cref="Card"/> with the given <paramref name="guid"/>.
    /// </summary>
    public static Card GetCardByGUID(string guid)
    {
        if (_cardsGUIDDictionary.TryGetValue(guid, out Card card))
        {
            return card;
        }

        Debug.LogWarning($"There is no Card with the GUID: \"{guid}\"");

        return null;
    }

    /// <summary>
    /// Returns a <see cref="CardTag"/> with the given <paramref name="name"/>.
    /// </summary>
    public static CardTag GetCardTagByName(string name)
    {
        if (_cardTagsNameDictionary.TryGetValue(name, out CardTag cardTag))
        {
            return cardTag;
        }

        Debug.LogWarning($"There is no CardTag with the name: \"{name}\"");

        return null;
    }

    /// <summary>
    /// Returns a <see cref="CardTag"/> with the given <paramref name="guid"/>.
    /// </summary>
    public static CardTag GetCardTagByGUID(string guid)
    {
        if (_cardTagsGUIDDictionary.TryGetValue(guid, out CardTag cardTag))
        {
            return cardTag;
        }

        Debug.LogWarning($"There is no CardTag with the GUID: \"{guid}\"");

        return null;
    }
}