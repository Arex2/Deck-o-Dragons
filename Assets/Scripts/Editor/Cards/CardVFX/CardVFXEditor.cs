using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(CardVFX))]
public class CardVFXEditor : Editor
{
    private ReorderableList _list;

    private CardVFX _target;

    private void OnEnable()
    {
        _target = (CardVFX)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();
        SerializedProperty iterator = serializedObject.GetIterator();
        bool enterChildren = true;
        while (iterator.NextVisible(enterChildren))
        {
            enterChildren = false;

            string propertyPath = iterator.propertyPath;

            if (propertyPath == "m_Script")
            {
                continue;
            }

            if (propertyPath == "autoDestroyTimer")
            {
                int vfxComponentCount = _target.GetComponentsInChildren<ICardVFXComponent>(true).Length;

                bool disable = vfxComponentCount > 0;

                using (new EditorGUI.DisabledScope(disable))
                {
                    EditorGUILayout.PropertyField(iterator, true);
                }

                if (disable)
                {
                    EditorGUILayout.HelpBox("Since this CardVFX has one or multiple ICardVFXComponent on it, Auto Destroy Timer will be ignored!\n\nThis CardVFX will instead be destroyed when the ICardVFXComponents coroutines end.", MessageType.Info);
                }
                continue;
            }

            if (propertyPath == "vfxTags")
            {
                if (_list == null)
                {
                    _list = new ReorderableList(serializedObject, serializedObject.FindProperty(propertyPath));
                    _list.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "VFX Tags", EditorStyles.boldLabel);
                    _list.drawElementCallback = (rect, index, isActive, isFocused) =>
                    {
                        rect.height -= EditorGUIUtility.standardVerticalSpacing;
                        EditorGUI.PropertyField(rect, _list.serializedProperty.GetArrayElementAtIndex(index), GUIContent.none);
                    };
                }

                _list.DoLayoutList();

                Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(-EditorGUIUtility.standardVerticalSpacing));

                rect.yMin -= EditorGUIUtility.singleLineHeight + 4;

                rect.width -= 68;

                if (GUI.Button(rect, "Add or remove VFX tags"))
                {
                    CardVFXManagerEditor.OpenPropertyEditor();
                }

                EditorGUILayout.Space();

                continue;
            }

            EditorGUILayout.PropertyField(iterator, true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
