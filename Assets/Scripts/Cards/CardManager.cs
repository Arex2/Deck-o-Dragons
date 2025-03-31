using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// The script that handles loading every <see cref="Card"/> object.
/// </summary>
// Script by Ruben
public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }

    public static bool Loaded { get; private set; }

    public static Card[] AllCards { get; private set; }

    [RuntimeInitializeOnLoadMethod]
    public static
#if !UNITY_EDITOR
            async
#endif
        void Initialize()
    {
        //CardManager cardManager = await Addressables.LoadAssetAsync<CardManager>(nameof(CardManager)).Task;

        GameObject newObj = new GameObject(nameof(CardManager), typeof(CardManager));
        DontDestroyOnLoad(newObj);

        Instance = newObj.GetComponent<CardManager>();

        AssetLabelReference labelReference = new AssetLabelReference();
        labelReference.labelString = "Cards";

#if !UNITY_EDITOR
            await
#endif
        List<Card> cards = new List<Card>();

        Addressables.LoadAssetsAsync<Card>(labelReference, (card) =>
        {
            card.OnLoad();
            cards.Add(card);
        })
#if UNITY_EDITOR
            // Instantly load in editor to prevent issues
            .WaitForCompletion();
#else
            .Task;
#endif

        AllCards = cards.ToArray();

        Loaded = true;
    }

    public static Coroutine StartStaticCoroutine(IEnumerator method)
    {
        return Instance.StartCoroutine(method);
    }

    public static void StopStaticCoroutine(Coroutine coroutine)
    {
        Instance.StopCoroutine(coroutine);
    }
}