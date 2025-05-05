using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomPropertyDrawer(typeof(CardVFXReference))]
public class CardVFXReferencePropertyDrawer : PropertyDrawer
{
    private static ReorderableList list
    {
        get
        {
            if (_cachedList == null)
            {
                _cachedList = new ReorderableList(default(SerializedObject), default(SerializedProperty), true, false, true, true);

                _cachedList.headerHeight = 0;
                _cachedList.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    rect.y += EditorGUIUtility.standardVerticalSpacing / 2;
                    rect.height -= EditorGUIUtility.standardVerticalSpacing;

                    EditorGUI.PropertyField(rect, _cachedList.serializedProperty.GetArrayElementAtIndex(index), GUIContent.none);
                };
            }

            return _cachedList;
        }
    }
    private static ReorderableList _cachedList;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty tagProperty = property.FindPropertyRelative("tag");

        Rect rect = position;
        rect.height = EditorGUIUtility.singleLineHeight;

        Rect prefixRect = CustomEditorUtility.GetPrefixRect(rect, out Rect remainerRect);

        GUIContent foldoutLabel = EditorGUI.BeginProperty(rect, label, property);

        property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(prefixRect, property.isExpanded, foldoutLabel, EditorStyles.foldout);
        EditorGUI.EndFoldoutHeaderGroup();

        EditorGUI.EndProperty();

        EditorGUI.PropertyField(remainerRect, tagProperty, GUIContent.none);

        if (!property.isExpanded)
        {
            return;
        }

        float offset = rect.height + EditorGUIUtility.standardVerticalSpacing;
        rect.y += offset;

        rect.height = position.height - offset;

        SerializedProperty vfxsProperty = property.FindPropertyRelative("vfxs");
        
        list.serializedProperty = vfxsProperty;
        list.DoList(rect);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty tagProperty = property.FindPropertyRelative("tag");

        float height = EditorGUIUtility.singleLineHeight;

        if (property.isExpanded)
        {
            list.serializedProperty = property.FindPropertyRelative("vfxs");
            height += list.GetHeight() + EditorGUIUtility.standardVerticalSpacing;
        }

        return height;
    }
    [InitializeOnLoadMethod]
    public static void InitContextualPropertyMenu()
    {
        EditorApplication.contextualPropertyMenu += OnContextualPropertyMenu;
    }

    private static void OnContextualPropertyMenu(GenericMenu menu, SerializedProperty property)
    {
        if (property.type != nameof(CardVFXReference))
        {
            return;
        }

        menu.AddItem(new GUIContent("Add or remove VFX Tags"), false, CardVFXManagerEditor.OpenPropertyEditor);
    }
}
