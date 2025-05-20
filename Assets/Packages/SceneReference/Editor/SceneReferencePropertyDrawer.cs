using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// The property drawer for <see cref="SceneReference"/>.
/// </summary>
[CustomPropertyDrawer(typeof(SceneReference))]
public class SceneReferencePropertyDrawer : PropertyDrawer
{
    private HashSet<string> _initialSet = new();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Begin property
        label = EditorGUI.BeginProperty(position, label, property);

        EditorGUI.BeginChangeCheck();

        // Scene Asset object field
        SerializedProperty assetProp = property.FindPropertyRelative("sceneAsset");

        assetProp.objectReferenceValue = EditorGUI.ObjectField(position, label, assetProp.objectReferenceValue, typeof(SceneAsset), false);

        // On change field
        bool inInitialSet = _initialSet.Contains(property.propertyPath);

        if (EditorGUI.EndChangeCheck() || !inInitialSet)
        {
            if (!inInitialSet)
            {
                _initialSet.Add(property.propertyPath);
            }

            SceneAsset asset = assetProp.objectReferenceValue as SceneAsset;

            // Set "sceneName" and "scenePath"
            property.FindPropertyRelative("sceneName").stringValue = asset == null ? string.Empty : asset.name;
            string assetPath = asset == null ? string.Empty : AssetDatabase.GetAssetPath(asset);
            property.FindPropertyRelative("scenePath").stringValue = assetPath;

            // Set build index
            SerializedProperty buildIndexProp = property.FindPropertyRelative("buildIndex");

            // Default to a value of -1 for invalid scenes
            buildIndexProp.intValue = -1;

            if (asset != null)
            {
                GUID guid = AssetDatabase.GUIDFromAssetPath(assetPath);

                int length = EditorBuildSettings.scenes.Length;
                for (int i = 0; i < length; i++)
                {
                    EditorBuildSettingsScene editorBuildSettingsScene = EditorBuildSettings.scenes[i];

                    if (guid != editorBuildSettingsScene.guid)
                    {
                        continue;
                    }

                    buildIndexProp.intValue = i;
                    break;
                }
            }
        }

        // End property
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}