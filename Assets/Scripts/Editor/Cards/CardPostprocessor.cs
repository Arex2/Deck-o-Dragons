using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Is responsible for saving a reference to every <see cref="Card"/> and <see cref="CardTag"/> object onto the <see cref="CardManager"/> object.
/// </summary>
// Script by Ruben
public class CardPostprocessor : AssetPostprocessor
{
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
    {
        if (didDomainReload)
        {
            Cache();
            return;
        }

        foreach (string path in importedAssets)
        {
            if (CheckAsset(path))
            {
                Cache();
                return;
            }
        }
    }

    private static void Cache()
    {
        CardManager cardManager = Resources.Load<GameObject>($"{CardManager.FOLDER}/{nameof(CardManager)}").GetComponentInChildren<CardManager>(true);

        if (cardManager == null)
        {
            Debug.LogWarning($"There is no \"{nameof(CardManager)}\" in the \"Resources/{CardManager.FOLDER}\" folder!");
            return;
        }

        using (SerializedObject serializedObject = new SerializedObject(cardManager))
        {
            serializedObject.UpdateIfRequiredOrScript();

            SerializedProperty cardsProp = serializedObject.FindProperty("allCards");
            SerializedProperty tagsProp = serializedObject.FindProperty("allTags");

            cardsProp.ClearArray();
            tagsProp.ClearArray();

            void Add(SerializedProperty array, Object obj)
            {
                int index = array.arraySize;

                array.InsertArrayElementAtIndex(index);
                array.GetArrayElementAtIndex(index).objectReferenceValue = obj;
            }

            // Get all cards and tags in the game
            foreach (string guid in AssetDatabase.FindAssets($"t:{nameof(Card)} t:{nameof(CardTag)}"))
            {
                Object obj = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(guid));

                if (obj is Card)
                {
                    Add(cardsProp, obj);
                }
                else if (obj is CardTag)
                {
                    Add(tagsProp, obj);
                }
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

    }

    private static bool CheckAsset(string assetPath)
    {
        // Ignore everything that isn't an "asset" file
        if (!assetPath.ToLower().EndsWith(".asset"))
        {
            return false;
        }

        // Check the type of the asset
        Type type = AssetDatabase.GetMainAssetTypeAtPath(assetPath);

        return type == typeof(Card) || type == typeof(CardTag) || type == typeof(CardComponent) || type == typeof(StatusEffect);
    }
}
