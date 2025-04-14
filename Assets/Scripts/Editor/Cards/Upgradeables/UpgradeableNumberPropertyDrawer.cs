using UnityEngine;
using UnityEditor;

/// <summary>
/// The custom property drawer for <see cref="UpgradeableNumber{T}"/>.
/// </summary>
// Script by Ruben
public abstract class UpgradeableNumberPropertyDrawer<T> : UpgradeableBasePropertyDrawer<T>
{
    private static GUIStyle _iconButtonStyle;

    private static readonly string[] _methods = new string[]
    {
        "Add (+)",
        "Subtract (-)",
        "Multiply (*)",
        "Divide (\\)",
        "Override (=)",
    };

    public override bool CanStack => true;

    public abstract T AddMethod(T a, T b);

    public abstract T SubtractMethod(T a, T b);

    public abstract T MultiplyMethod(T a, T b);

    public abstract T DivideMethod(T a, T b);

    public override void AddMoreMenuOptions(GenericMenu menu, SerializedProperty property)
    {
        AddMethodOptions(menu, "Calculation Method/", property.FindPropertyRelative("method"));
    }

    private void AddMethodOptions(GenericMenu menu, string folder, SerializedProperty property)
    {
        int enumValueIndex = property.enumValueIndex;

        int length = _methods.Length;
        for (int i = 0; i < length; i++)
        {
            int index = i;

            menu.AddItem(new GUIContent(folder + _methods[i]), enumValueIndex == i, () =>
            {
                property.enumValueIndex = index;

                property.serializedObject.ApplyModifiedProperties();
            });
        }
    }

    public override void DrawTier(Rect rect, SerializedProperty property, GUIContent label, bool isDowngrade, ref T result)
    {
        label = EditorGUI.BeginProperty(rect, label, property);

        SerializedProperty valueProp = property.FindPropertyRelative("value");
        SerializedProperty methodProp = property.FindPropertyRelative("method");

        Rect prefixRect = CustomEditorUtility.GetPrefixRect(rect);

        Rect iconRect = prefixRect;
        iconRect.width = 16;
        iconRect.x += prefixRect.width - iconRect.width;

        Event evt = Event.current;

        if (iconRect.Contains(evt.mousePosition) && evt.type == EventType.MouseDown && evt.button == 0)
        {
            GenericMenu menu = new GenericMenu();

            AddMethodOptions(menu, "", methodProp);

            menu.DropDown(iconRect);

            evt.Use();
        }

        UpgradeableFloat.Method method = (UpgradeableFloat.Method)methodProp.enumValueIndex;

        // Result label
        Rect resultRect = rect;

        resultRect.width = 60;
        resultRect.x += rect.width - resultRect.width - 14;

        rect.width -= resultRect.width;

        resultRect.width += 14;

        T propValue = GetPropValue(valueProp);

        switch (method)
        {
            case UpgradeableFloat.Method.Add:
                result = AddMethod(result, propValue);
                break;

            case UpgradeableFloat.Method.Subtract:
                result = SubtractMethod(result, propValue);
                break;

            case UpgradeableFloat.Method.Multiply:
                result = MultiplyMethod(result, propValue);
                break;

            case UpgradeableFloat.Method.Divide:
                result = DivideMethod(result, propValue);
                break;

            case UpgradeableFloat.Method.Override:
                result = propValue;
                break;
        }

        CustomEditorUtility.DoFadedLabel(resultRect, "= " + result);

        DrawProp(rect, valueProp, label);

        string icon = method switch
        {
            UpgradeableFloat.Method.Add => "+",
            UpgradeableFloat.Method.Subtract => "-",
            UpgradeableFloat.Method.Multiply => "*",
            UpgradeableFloat.Method.Divide => "/",
            _ => "=",
        };

        if (_iconButtonStyle == null)
        {
            _iconButtonStyle = new GUIStyle(EditorStyles.iconButton);

            _iconButtonStyle.alignment = TextAnchor.MiddleCenter;
        }

        Color startColor = GUI.contentColor;
        Color color = isDowngrade ?
            new Color(1, 0.5f, 0.5f)
            :
            new Color(0.5f, 1, 0.5f);
        color.a = 0.5f;

        GUI.contentColor = color;

        GUI.Button(iconRect, icon, _iconButtonStyle);

        //DoFadedLabel(iconRect, icon, false);

        GUI.contentColor = startColor;

        EditorGUI.EndProperty();
    }

    public override bool MatchesTier(string propertyType)
    {
        return propertyType == "NumberTier";
    }
}
