using System.Security;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

/// <summary>
/// The custom editor for <see cref="CardVFXManager"/>.
/// </summary>
// Script by Ruben
[CustomEditor(typeof(CardVFXManager))]
public class CardVFXManagerEditor : Editor
{
    public static SerializedObject Instance
    {
        get
        {
            if (_instance == null)
            {
                CardVFXManager cardVFXManager = Resources.Load<GameObject>($"{CardVFXManager.FOLDER}/{nameof(CardVFXManager)}").GetComponentInChildren<CardVFXManager>(true);

                if (cardVFXManager == null)
                {
                    Debug.LogWarning($"There is no \"{nameof(CardVFXManager)}\" in the \"Resources/{CardVFXManager.FOLDER}\" folder!");
                }
                else
                {
                    _instance = new SerializedObject(cardVFXManager);
                }
            }

            return _instance;
        }
    }
    private static SerializedObject _instance;

    private ReorderableList _list;

    public static void OpenPropertyEditor()
    {
        if (Instance == null)
        {
            return;
        }

        EditorUtility.OpenPropertyEditor(Instance.targetObject);

        Instance.UpdateIfRequiredOrScript();
        Instance.FindProperty("tags").isExpanded = true;
        Instance.FindProperty("cardVFXPrefabs").isExpanded = false;
        Instance.ApplyModifiedPropertiesWithoutUndo();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        if (_list == null)
        {
            _list = new ReorderableList(serializedObject, serializedObject.FindProperty("tags"));
            _list.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "VFX Tags", EditorStyles.boldLabel);
            _list.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                rect.height -= EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(rect, _list.serializedProperty.GetArrayElementAtIndex(index), GUIContent.none);
            };
        }

        _list.DoLayoutList();

        EditorGUILayout.Space();

        EditorGUI.BeginDisabledGroup(true);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("cardVFXPrefabs"));

        EditorGUI.EndDisabledGroup();

        serializedObject.ApplyModifiedProperties();
    }
}
