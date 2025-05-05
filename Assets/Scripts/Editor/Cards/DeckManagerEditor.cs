using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(DeckManager))]
public class DeckManagerEditor : Editor
{
    private ReorderableList _list;

    private DeckManager _target;

    private void OnEnable()
    {
        _target = (DeckManager)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();

        using (new EditorGUI.DisabledScope(true))
        {
            if (_list == null)
            {
                _list = new ReorderableList(_target.Deck, typeof(Card), false, true, false, false);
                _list.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Deck", EditorStyles.boldLabel);
                _list.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    rect.height -= EditorGUIUtility.standardVerticalSpacing;
                    EditorGUI.ObjectField(rect, GUIContent.none, (Card)_list.list[index], typeof(Card), false);
                };
            }

            EditorGUILayout.Space();

            _list.DoLayoutList();
        }
    }
}
