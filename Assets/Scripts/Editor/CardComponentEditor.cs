using UnityEngine;
using UnityEditor;

/// <summary>
/// The custom editor script for <see cref="CardComponent"/>.
/// </summary>
// Script by Ruben
[CustomEditor(typeof(CardComponent), true)]
public class CardComponentEditor : Editor
{
    private SerializedProperty _cardProp;

    private void OnEnable()
    {
        _cardProp = serializedObject.FindProperty("card");
    }

    public override void OnInspectorGUI()
    {
        bool isSubAsset = AssetDatabase.IsSubAsset(target);

        string oldName = target.name;
        target.name = EditorGUILayout.DelayedTextField("Name", target.name);

        EditorGUILayout.Space();

        serializedObject.UpdateIfRequiredOrScript();

        SerializedProperty iterator = serializedObject.GetIterator();

        bool enterChildren = true;
        while (iterator.NextVisible(enterChildren))
        {
            switch (iterator.propertyPath)
            {
                case "m_Script":
                    continue;
            }

            EditorGUILayout.PropertyField(iterator, true);

            enterChildren = false;
        }

        Object parentAsset = null;
        if (isSubAsset)
        {
            parentAsset = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(target));
            _cardProp.objectReferenceValue = parentAsset;
        }

        /*
        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        Rect buttonRect = GUILayoutUtility.GetRect(240, 25);

        bool isChild = AssetDatabase.IsSubAsset(target);

        using (new EditorGUI.DisabledScope(!isChild))
        {
            if (GUI.Button(buttonRect, "Remove this Card Component") && isChild)
            {
                Object mainObj = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(target));

                Undo.RecordObject(mainObj, "Deleted object");

                string name = target.name;

                Undo.DestroyObjectImmediate(target);

                //AssetDatabase.RemoveObjectFromAsset(target);
                Undo.SetCurrentGroupName($"Delete \"{name}\"");

                EditorUtility.SetDirty(mainObj);
                AssetDatabase.SaveAssetIfDirty(mainObj);

                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(mainObj), ImportAssetOptions.ForceUpdate);

                return;
            }
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        */

        serializedObject.ApplyModifiedProperties();

        if (isSubAsset && oldName != target.name)
        {
            CardEditor.SaveAsset(parentAsset);
        }
    }
}