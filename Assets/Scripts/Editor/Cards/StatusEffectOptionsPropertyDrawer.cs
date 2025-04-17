using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// The custom property drawer for <see cref="StatusEffectOptions"/>.
/// </summary>
// Script by Ruben
[CustomPropertyDrawer(typeof(StatusEffectOptions))]
public class StatusEffectOptionsPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect rect = position;

        rect.height = EditorGUIUtility.singleLineHeight;

        void NextHeight()
        {
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
            rect.height = EditorGUIUtility.singleLineHeight;
        }

        Rect prefixRect = CustomEditorUtility.GetPrefixRect(rect);
        
        SerializedProperty statusEffectProp = property.FindPropertyRelative("statusEffect");

        Rect remainderRect = rect;
        remainderRect.xMin = prefixRect.xMax;

        EditorGUI.PropertyField(remainderRect, statusEffectProp, GUIContent.none);

        property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(prefixRect, property.isExpanded, property.displayName, EditorStyles.foldout);
        EditorGUI.EndFoldoutHeaderGroup();

        if (!property.isExpanded)
        {
            EditorGUI.EndProperty();
            return;
        }

        rect.y += EditorGUIUtility.standardVerticalSpacing;

        SerializedProperty potencyProp = property.FindPropertyRelative("potency");
        SerializedProperty durationProp = property.FindPropertyRelative("duration");

        Rect bgRect = position;
        bgRect.yMin = rect.yMax;

        CustomEditorUtility.DrawBGBox(bgRect);

        rect.y += CustomEditorUtility.SPACING / 2;

        EditorGUI.indentLevel++;

        NextHeight();

        rect.height = EditorGUI.GetPropertyHeight(potencyProp);

        StatusEffect statusEffect = statusEffectProp.objectReferenceValue as StatusEffect;

        EditorGUI.BeginDisabledGroup(statusEffect == null);

        label.text = potencyProp.displayName;

        if (statusEffect != null)
        {
            label.text += $" ({statusEffect.PotencyName})";
        }

        EditorGUI.BeginDisabledGroup(statusEffect != null && !statusEffect.HasPotency);

        EditorGUI.PropertyField(rect, potencyProp, label);

        EditorGUI.EndDisabledGroup();

        NextHeight();

        rect.height = EditorGUI.GetPropertyHeight(durationProp);

        label.text = durationProp.displayName;

        if (statusEffect != null)
        {
            label.text += $" ({statusEffect.DurationName})";
        }

        EditorGUI.PropertyField(rect, durationProp, label);

        EditorGUI.EndDisabledGroup();

        EditorGUI.indentLevel--;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight;

        if (property.isExpanded)
        {
            height += EditorGUIUtility.standardVerticalSpacing * 3f;
            height += CustomEditorUtility.SPACING * 1.5f;

            SerializedProperty potencyProp = property.FindPropertyRelative("potency");
            SerializedProperty durationProp = property.FindPropertyRelative("duration");

            height += EditorGUI.GetPropertyHeight(potencyProp);
            height += EditorGUI.GetPropertyHeight(durationProp);
        }

        return height;
    }
}
