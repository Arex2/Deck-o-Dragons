using UnityEngine;
using UnityEditor;

/// <summary>
/// The custom property drawer for <see cref="UpgradeableFloat"/>.
/// </summary>
// Script by Ruben
[CustomPropertyDrawer(typeof(UpgradeableFloat))]
public class UpgradeableFloatPropertyDrawer : UpgradeableNumberPropertyDrawer<float>
{
    protected override float AddMethod(float a, float b)
    {
        return a + b;
    }

    protected override float SubtractMethod(float a, float b)
    {
        return a - b;
    }

    protected override float MultiplyMethod(float a, float b)
    {
        return a * b;
    }

    protected override float DivideMethod(float a, float b)
    {
        if (b == 0)
        {
            Debug.LogWarning("Diving by zero!");
            return 0;
        }

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

    public override float GetLevelHeight(SerializedProperty prop)
    {
        return GetPropHeight(prop);
    }
}
