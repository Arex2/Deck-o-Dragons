using UnityEditor;
using UnityEngine;

/// <summary>
/// The custom property drawer for <see cref="TargetFilter"/>.
/// </summary>
// Script by Ruben
[CustomPropertyDrawer(typeof(TargetFilter))]
public class TargetFilterPropertyDrawer : PropertyDrawer
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
