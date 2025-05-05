using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SingletonMode(true)]
public class CardVFXManager : Singleton<CardVFXManager>
{
    private static readonly Dictionary<string, CardVFX[]> _cardVFXTagDictionary = new();

    public static readonly List<CardVFX> ActiveVFX = new();
    public static int ActiveVFXCount { get; private set; }

    [SerializeField] private string[] tags;

    [HideInInspector]
    [SerializeField] private CardVFX[] cardVFXPrefabs;

    protected override void Awake()
    {
        base.Awake();

        // Caching by doing whatever this is
        Dictionary<string, List<CardVFX>> temp = new();

        foreach (CardVFX cardVFX in cardVFXPrefabs)
        {
            foreach (string vfxTag in cardVFX.VFXTags)
            {
                string formattedTag = FormatVFXTag(vfxTag);

                if (!temp.ContainsKey(formattedTag))
                {
                    temp.Add(formattedTag, new());
                }

                temp[formattedTag].Add(cardVFX);
            }
        }

        foreach (var pair in temp)
        {
            _cardVFXTagDictionary.Add(pair.Key, pair.Value.ToArray());
        }
    }

    public static string FormatVFXTag(string vfxTag)
    {
        return vfxTag.Trim().ToLower();
    }

    public static List<CardVFX> SpawnVFX(IEnumerable<CardVFX> enumerable) => SpawnVFX(enumerable, Vector3.zero);
    public static List<CardVFX> SpawnVFX(IEnumerable<CardVFX> enumerable, Vector3 position)
    {
        List<CardVFX> result = new();

        foreach (CardVFX vfx in enumerable)
        {
            result.Add(SpawnVFX(vfx, position));
        }

        return result;
    }

    public static CardVFX SpawnVFX(CardVFX cardVFX) => SpawnVFX(cardVFX, Vector3.zero);
    public static CardVFX SpawnVFX(CardVFX cardVFX, Vector3 position)
    {
        if (cardVFX == null)
        {
            return null;
        }

        CardVFX Spawn() => Instantiate(cardVFX, position, Quaternion.identity);

        float spawnDelay = cardVFX.SpawnDelay;

        CardVFX newCardVFX;

        if (spawnDelay > 0)
        {
            bool startCardVFXActive = cardVFX.gameObject.activeSelf;
            cardVFX.gameObject.SetActive(false);

            newCardVFX = Spawn();

            cardVFX.gameObject.SetActive(startCardVFXActive);
            Instance.StartCoroutine(VFXCoroutine(newCardVFX, spawnDelay));
        }
        else
        {
            newCardVFX = Spawn();
        }

        ActiveVFX.Add(newCardVFX);
        ActiveVFXCount++;

        return newCardVFX;
    }

    private static IEnumerator VFXCoroutine(CardVFX cardVFX, float delay)
    {
        yield return new WaitForSeconds(delay);

        cardVFX.gameObject.SetActive(true);
    }

    public static CardVFX[] GetVFXWithTag(string vfxTag, bool format = true)
    {
        if (format)
        {
            vfxTag = FormatVFXTag(vfxTag);
        }

        if (_cardVFXTagDictionary.TryGetValue(vfxTag, out CardVFX[] cardVFXs))
        {
            return cardVFXs;
        }

        return null;
    }

    public static void OnDestroy(CardVFX cardVFX)
    {
        if (ActiveVFX.Remove(cardVFX))
        {
            ActiveVFXCount--;
        }
    }
}
