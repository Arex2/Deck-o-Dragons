using UnityEngine;
using UnityEditor;

/// <summary>
/// The custom property drawer for <see cref="UpgradeableFloat"/>.
/// </summary>
// Script by Ruben
[CustomPropertyDrawer(typeof(UpgradeableFloat))]
public class UpgradeableFloatPropertyDrawer : UpgradeableNumberPropertyDrawer<float>
{
    public override float AddMethod(float a, float b)
    {
        return a + b;
    }

    public override float SubtractMethod(float a, float b)
    {
        return a - b;
    }

    public override float MultiplyMethod(float a, float b)
    {
        return a * b;
    }

    public override float DivideMethod(float a, float b)
    {
        return a / b;
    }

    public override float GetPropValue(SerializedProperty prop)
    {
        return prop.floatValue;
    }

    public override void DrawProp(Rect rect, SerializedProperty prop, GUIContent label)
    {
        prop.floatValue = EditorGUI.FloatField(rect, label, prop.floatValue);
    }

    public override bool MatchesThisClass(string propertyType)
    {
        return propertyType == nameof(UpgradeableFloat);
    }

    public override float GetPropHeight(SerializedProperty prop)
    {
        return EditorGUIUtility.singleLineHeight;
    }

    public override float GetTierHeight(SerializedProperty prop)
    {
        return GetPropHeight(prop);
    }
}
