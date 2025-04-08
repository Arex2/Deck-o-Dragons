using UnityEngine;
using UnityEditor;

/// <summary>
/// The custom editor for <see cref="GUIDScriptableObject"/>.
/// </summary>
// Script by Ruben
[CustomEditor(typeof(GUIDScriptableObject), true)]
public class GUIDScriptableObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        SerializedProperty iterator = serializedObject.GetIterator();
        bool enterChildren = true;

        while (iterator.NextVisible(enterChildren))
        {
            enterChildren = false;

            switch (iterator.propertyPath)
            {
                case "m_Script":
                    continue;

                case "guid":
                    HandleGUIDField(iterator, target);
                    continue;
            }

            EditorGUILayout.PropertyField(iterator);
        }

        serializedObject.ApplyModifiedProperties();
    }

    public static void HandleGUIDField(SerializedProperty property, Object target)
    {
        property.stringValue = AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(target)).ToString();
    }
}
