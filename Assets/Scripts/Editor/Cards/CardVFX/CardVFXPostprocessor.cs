using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Is responsible for saving a reference to every <see cref="CardVFX"/> onto the <see cref="CardVFXManager"/> object.
/// </summary>
// Script by Ruben
public class CardVFXPostprocessor : AssetPostprocessor
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
        SerializedObject serializedObject = CardVFXManagerEditor.Instance;

        if (serializedObject == null)
        {
            return;
        }

        serializedObject.UpdateIfRequiredOrScript();

        SerializedProperty cardVFXProp = serializedObject.FindProperty("cardVFXPrefabs");

        cardVFXProp.ClearArray();

        // Cache all card VFX prefabs
        foreach (string guid in AssetDatabase.FindAssets($"t:{nameof(GameObject)}"))
        {
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));

            CardVFX cardVFX = obj.GetComponentInChildren<CardVFX>(true);

            if (cardVFX == null)
            {
                continue;
            }

            int index = cardVFXProp.arraySize;

            cardVFXProp.InsertArrayElementAtIndex(index);
            cardVFXProp.GetArrayElementAtIndex(index).objectReferenceValue = cardVFX;
        }

        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private static bool CheckAsset(string assetPath)
    {
        // Ignore everything that isn't a "prefab" file
        if (!assetPath.ToLower().EndsWith(".prefab"))
        {
            return false;
        }

        // Check the type of the asset
        Type type = AssetDatabase.GetMainAssetTypeAtPath(assetPath);

        return type == typeof(GameObject);
    }
}
