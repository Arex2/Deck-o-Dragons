using UnityEngine;
using UnityEditor;

/// <summary>
/// The custom property drawer for <see cref="UpgradeableInt"/>.
/// </summary>
// Script by Ruben
[CustomPropertyDrawer(typeof(UpgradeableInt))]
public class UpgradeableIntPropertyDrawer : UpgradeableNumberPropertyDrawer<int>
{
    protected override int AddMethod(int a, int b)
    {
        return a + b;
    }

    protected override int SubtractMethod(int a, int b)
    {
        return a - b;
    }

    protected override int MultiplyMethod(int a, int b)
    {
        return a * b;
    }

    protected override int DivideMethod(int a, int b)
    {
        return a / b;
    }

    public override int GetPropValue(SerializedProperty prop)
    {
        return prop.intValue;
    }

    public override void DrawProp(Rect rect, SerializedProperty prop, GUIContent label)
    {
        prop.intValue = EditorGUI.IntField(rect, label, prop.intValue);
    }

    public override bool MatchesThisClass(string propertyType)
    {
        return propertyType == nameof(UpgradeableInt);
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
