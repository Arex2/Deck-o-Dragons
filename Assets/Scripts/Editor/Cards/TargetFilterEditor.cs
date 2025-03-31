using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TargetFilter))]
public class TargetFilterEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);

        Rect remainderRect = EditorGUI.PrefixLabel(position, label);

        SerializedProperty teamProp = property.FindPropertyRelative("team");
        SerializedProperty modeProp = property.FindPropertyRelative("mode");

        Rect teamRect = remainderRect;
        teamRect.xMax -= remainderRect.width / 2;

        EditorGUI.PropertyField(teamRect, teamProp, GUIContent.none);

        Rect modeRect = remainderRect;
        modeRect.xMin += remainderRect.width / 2;

        EditorGUI.PropertyField(modeRect, modeProp, GUIContent.none);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}
