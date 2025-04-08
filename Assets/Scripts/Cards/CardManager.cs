using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The script that handles loading and keeping track of every <see cref="Card"/>.
/// </summary>
// Script by Ruben
[SingletonMode(true)]
public class CardManager : Singleton<CardManager>
{
    public static Card[] AllCards { get; private set; }
    public static CardTag[] AllTags { get; private set; }

    private static readonly Dictionary<CardTag, List<Card>> _cardsTagDictionary = new();
    private static readonly Dictionary<string, Card> _cardsNameDictionary = new();
    private static readonly Dictionary<string, Card> _cardsGUIDDictionary = new();
    private static readonly Dictionary<string, CardTag> _cardTagsNameDictionary = new();
    private static readonly Dictionary<string, CardTag> _cardTagsGUIDDictionary = new();

    protected override void Awake()
    {
        base.Awake();

        AllCards = Resources.LoadAll<Card>("Cards");
        AllTags = Resources.LoadAll<CardTag>("Cards/Tags");

        foreach (CardTag tag in AllTags)
        {
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

        foreach (Card card in AllCards)
        {
            foreach (CardTag tag in card.Tags)
            {
                if (!_cardsTagDictionary.ContainsKey(tag))
                {
                    _cardsTagDictionary.Add(tag, new());
                }

                _cardsTagDictionary[tag].Add(card);
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

            _cardsGUIDDictionary.Add(card.GUID, card);
        }

        foreach (Card card in AllCards)
        {
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
    /// Returns a list of <see cref="Card"/>s that have the attached <paramref name="tag"/>.
    /// </summary>
    public static List<Card> GetCardsByTag(CardTag tag)
    {
        if (_cardsTagDictionary.TryGetValue(tag, out List<Card> cards))
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
    /// Returns a <see cref="Card"/> with the given <paramref name="name"/>.
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
    /// Returns a <see cref="Card"/> with the given <paramref name="guid"/>.
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