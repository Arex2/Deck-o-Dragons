using System;
using UnityEngine;
using UnityEditor;

/// <summary>
/// The custom property drawer for <see cref="Optional{T}"/>.
/// </summary>
[CustomPropertyDrawer(typeof(Optional<>))]
public class OptionalPropertyDrawer : PropertyDrawer
{
    public const float TOGGLE_WIDTH = 15;
    public const float SPACING = 5;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        DrawProperty(position, property, label);
    }

    public static void DrawProperty(Rect position, SerializedProperty property, GUIContent label, bool forceEnable = false, Action<Rect, SerializedProperty> valueDrawMethod = null)
    {
        Rect toggleRect = GetToggleRect(position, out Rect remainderRect);

        Rect togglePropertyRect = toggleRect;

        togglePropertyRect.width += SPACING * 2;
        togglePropertyRect.x -= SPACING;

        label = EditorGUI.BeginProperty(togglePropertyRect, label, property);

        SerializedProperty enabledProp = property.FindPropertyRelative("Enabled");
        enabledProp.boolValue = GUI.Toggle(toggleRect, enabledProp.boolValue, GUIContent.none, EditorStyles.toggle);

        EditorGUI.EndProperty();

        EditorGUI.BeginDisabledGroup(!enabledProp.boolValue && !forceEnable);

        SerializedProperty valueProp = property.FindPropertyRelative("Value");

        if (valueDrawMethod != null)
        {
            valueDrawMethod.Invoke(remainderRect, valueProp);
        }
        else
        {
            EditorGUI.PropertyField(remainderRect, valueProp, label, true);
        }

        EditorGUI.EndDisabledGroup();
    }

    public static Rect GetToggleRect(Rect position, out Rect remainderRect)
    {
        Rect result = position;

        result.width = TOGGLE_WIDTH;
        result.height = EditorGUIUtility.singleLineHeight;
        result.x += position.width - result.width;

        remainderRect = position;

        remainderRect.width -= result.width + SPACING;

        return result;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("Value"));
    }
}