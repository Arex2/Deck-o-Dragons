using UnityEngine;
using UnityEditor;

/// <summary>
/// The editor script for <see cref="UIDissolve"/>.
/// </summary>
// Script by Ruben
[CustomEditor(typeof(UIDissolve))]
public class UIDissolveEditor : Editor
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

                case "rotation":
                    iterator.floatValue = EditorGUILayout.FloatField(iterator.displayName, iterator.floatValue * Mathf.Rad2Deg) * Mathf.Deg2Rad;
                    continue;

                default:
                    EditorGUILayout.PropertyField(iterator, true);
                    continue;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
