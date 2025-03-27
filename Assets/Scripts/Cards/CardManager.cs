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

        Debug.Log("Began loading...");

#if !UNITY_EDITOR
            await
#endif
        Addressables.LoadAssetsAsync<Card>(labelReference, (card) =>
        {
            card.OnLoad();

            Debug.Log("Loaded \"" + card.name + "\"");
        })
#if UNITY_EDITOR
            // Instantly load in editor to prevent issues
            .WaitForCompletion();
#else
            .Task;
#endif

        Loaded = true;
        Debug.Log("Loaded all cards!");
    }
}