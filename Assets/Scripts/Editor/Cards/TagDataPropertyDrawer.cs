using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

[CustomPropertyDrawer(typeof(TagData))]
public class TagDataPropertyDrawer : PropertyDrawer
{
    private static ReorderableList _list;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect foldoutRect = position;
        foldoutRect.height = EditorGUIUtility.singleLineHeight;

        Rect arraySizeRect = foldoutRect;

        arraySizeRect.xMin = arraySizeRect.xMax - 50;
        foldoutRect.xMax -= arraySizeRect.width;

        property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(foldoutRect, property.isExpanded, new GUIContent(property.displayName));
        EditorGUI.EndFoldoutHeaderGroup();

        SerializedProperty dataProp = property.FindPropertyRelative("data");
        dataProp.arraySize = EditorGUI.DelayedIntField(arraySizeRect, dataProp.arraySize);

        if (!property.isExpanded)
        {
            EditorGUI.EndProperty();
            return;
        }

        Rect listRect = position;
        listRect.yMin += foldoutRect.height + EditorGUIUtility.standardVerticalSpacing;

        InitList(dataProp);

        _list.DoList(listRect);
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight;

        if (!property.isExpanded)
        {
            return height;
        }

        height += EditorGUIUtility.standardVerticalSpacing;

        InitList(property.FindPropertyRelative("data"));

        height += _list.GetHeight();

        return height;
    }

    private static void InitList(SerializedProperty property)
    {
        if (_list == null)
        {
            _list = new ReorderableList(property.serializedObject, property, true, false, true, true);
            _list.drawElementCallback = DrawElementCallback;
            _list.elementHeight = EditorGUIUtility.singleLineHeight;
        }
        else
        {
            _list.serializedProperty = property;
        }
    }

    private static void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        rect.height -= EditorGUIUtility.standardVerticalSpacing;

        SerializedProperty property = _list.serializedProperty.GetArrayElementAtIndex(index);

        SerializedProperty tagProp = property.FindPropertyRelative("tag");
        SerializedProperty potencyProp = property.FindPropertyRelative("potency");

        CardTag obj = tagProp.objectReferenceValue as CardTag;

        Rect potencyRect = rect;
        potencyRect.xMin = potencyRect.xMax - 80;

        rect.xMax -= potencyRect.width + CustomEditorUtility.SPACING;

        using (new EditorGUI.DisabledScope(obj == null || !obj.HasPotency))
        {
            EditorGUI.PropertyField(potencyRect, potencyProp, GUIContent.none);
        }

        EditorGUI.PropertyField(rect, tagProp, GUIContent.none);
    }
}
