using UnityEngine;
using UnityEditor;

/// <summary>
/// The custom property drawer for <see cref="Upgradeable{T}"/>.
/// </summary>
// Script by Ruben
[CustomPropertyDrawer(typeof(Upgradeable<>))]
public class UpgradeablePropertyDrawer : UpgradeableBasePropertyDrawer<object>
{
    public override bool CanStack => false;

    public override void DrawProp(Rect rect, SerializedProperty prop, GUIContent label)
    {
        EditorGUI.PropertyField(rect, prop, label);
    }

    public override void DrawTier(Rect rect, SerializedProperty property, GUIContent label, bool isDowngrade, ref object result)
    {
        label = EditorGUI.BeginProperty(rect, label, property);

        SerializedProperty valueProp = property.FindPropertyRelative("value");

        EditorGUI.EndProperty();
        DrawProp(rect, valueProp, label);
    }

    public override object GetPropValue(SerializedProperty prop)
    {
        return prop.boxedValue;
    }

    public override bool MatchesThisClass(string propertyType)
    {
        return propertyType == typeof(Upgradeable<>).Name;
    }

    public override bool MatchesTier(string propertyType)
    {
        return propertyType == "Tier";
    }

    public override void AddMoreMenuOptions(GenericMenu menu, SerializedProperty property)
    {

    }

    public override float GetPropHeight(SerializedProperty prop)
    {
        return EditorGUI.GetPropertyHeight(prop);
    }

    public override float GetTierHeight(SerializedProperty prop)
    {
        return GetPropHeight(prop.FindPropertyRelative("value"));
    }
}
