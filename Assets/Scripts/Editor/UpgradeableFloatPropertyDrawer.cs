using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Media;

[CustomPropertyDrawer(typeof(UpgradeableFloat))]
public class UpgradeableFloatPropertyDrawer : PropertyDrawer
{
    private const float SPACING = 8;

    private static readonly GUIContent _emptyContent = new GUIContent(" ");
    private static GUIStyle _italicLabelStyle;
    private static GUIStyle _iconButtonStyle;

    private static readonly FieldInfo _genericMenuItemsField = typeof(GenericMenu).GetField("m_MenuItems", BindingFlags.NonPublic  | BindingFlags.Instance);
    private static readonly Type _genericMenuItemType = typeof(GenericMenu).GetNestedType("MenuItem", BindingFlags.NonPublic);
    private static readonly FieldInfo _genericMenuItemContentField = _genericMenuItemType.GetField("content", BindingFlags.Public | BindingFlags.Instance);
    private static readonly MethodInfo _loopThroughAllMenuItemsInList = typeof(UpgradeableFloatPropertyDrawer).GetMethod(nameof(LoopThroughAllMenuItemsInList), BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(_genericMenuItemType);

    private static readonly string[] _methods = new string[]
    {
        "Add (+)",
        "Subtract (-)",
        "Multiply (*)",
        "Divide (\\)",
        "Override (=)",
    };

    private float _oldFloatValue;
    private bool _shouldExpand;

    public UpgradeableFloatPropertyDrawer() : base()
    {
        EditorApplication.contextualPropertyMenu += OnPropertyContextMenu;
    }

    ~UpgradeableFloatPropertyDrawer()
    {
        EditorApplication.contextualPropertyMenu -= OnPropertyContextMenu;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty baseProp = property.FindPropertyRelative("baseValue");

        Rect rect = position;
        rect.height = EditorGUIUtility.singleLineHeight;

        Event evt = Event.current;

        void NextHeight()
        {
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
        }

        GUIContent propertyLabel = EditorGUI.BeginProperty(rect, label, property);

        Rect foldoutPosition = baseProp.isExpanded ? rect : GetPrefixRect(rect);

        /*
        float offset = EditorStyles.inspectorDefaultMargins.padding.left - EditorStyles.inspectorDefaultMargins.padding.right;
        foldoutPosition.x -= offset;
        foldoutPosition.width += offset;
        */

        /*
        if (!baseProp.isExpanded)
        {
            if (evt.type == EventType.MouseDown && evt.button == 0)
            {
                _shouldExpand = true;

                _oldFloatValue = baseProp.floatValue;
            }

            if (evt.type == EventType.MouseUp && evt.button == 0)
            {
                if (_shouldExpand && foldoutPosition.Contains(evt.mousePosition))
                {
                    baseProp.isExpanded = !baseProp.isExpanded;
                    evt.Use();
                }
            }

            // Messy af
            baseProp.floatValue = EditorGUI.FloatField(rect, new GUIContent("    " + propertyLabel.text), baseProp.floatValue);

            if (_oldFloatValue != baseProp.floatValue)
            {
                _shouldExpand = false;
            }
        }
        */

        if (evt.type == EventType.MouseDown && evt.button == 0)
        {
            _shouldExpand = true;

            _oldFloatValue = baseProp.floatValue;
        }

        if (evt.type == EventType.MouseUp && evt.button == 0)
        {
            if (_shouldExpand && foldoutPosition.Contains(evt.mousePosition))
            {
                baseProp.isExpanded = !baseProp.isExpanded;
                evt.Use();
            }
        }

        baseProp.floatValue = EditorGUI.FloatField(rect, new GUIContent("    " + propertyLabel.text), baseProp.floatValue);

        if (_oldFloatValue != baseProp.floatValue)
        {
            _shouldExpand = false;
        }

        baseProp.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(foldoutPosition, baseProp.isExpanded, /*baseProp.isExpanded ? propertyLabel : */_emptyContent, EditorStyles.foldout);
        EditorGUI.EndFoldoutHeaderGroup();

        EditorGUI.EndProperty();

        if (!baseProp.isExpanded)
        {
            return;
        }

        Rect remainderRect = position;
        remainderRect.yMin += rect.height + SPACING / 2;

        DrawBGBox(remainderRect);

        rect.y += SPACING;

        EditorGUI.indentLevel++;

        NextHeight();

        // Upgrades
        SerializedProperty upgradesProp = property.FindPropertyRelative("upgrades");
        int upgradesArraySize = upgradesProp.arraySize;

        if (upgradesArraySize > 0)
        {
            for (int i = upgradesArraySize - 1; i >= 0; i--)
            {
                DrawTier(rect, upgradesProp.GetArrayElementAtIndex(i), new GUIContent("Upgrade " + (i + 1)), false);

                NextHeight();
            }
        }
        else
        {
            DoFadedLabel(rect, "This float has no upgrades. Right click to add some!");

            if (rect.Contains(evt.mousePosition) && evt.type == EventType.ContextClick)
            {
                GenericMenu menu = new GenericMenu();

                AddUpgradeAndDowngradeOptions(menu, property);

                menu.ShowAsContext();

                evt.Use();
            }

            NextHeight();
        }

        rect.y += SPACING;

        propertyLabel = EditorGUI.BeginProperty(rect, new GUIContent("Base Value"), baseProp);
        baseProp.floatValue = EditorGUI.FloatField(rect, propertyLabel, baseProp.floatValue);
        EditorGUI.EndProperty();

        NextHeight();

        rect.y += SPACING;

        // Downgrades
        SerializedProperty downgradesProp = property.FindPropertyRelative("downgrades");
        int downgradesArraySize = downgradesProp.arraySize;

        if (downgradesArraySize > 0)
        {
            for (int i = 0; i < downgradesArraySize; i++)
            {
                DrawTier(rect, downgradesProp.GetArrayElementAtIndex(i), new GUIContent("Downgrade " + (i + 1)), true);

                NextHeight();
            }
        }
        else
        {
            DoFadedLabel(rect, "This float has no downgrades. Right click to add some!");

            if (rect.Contains(evt.mousePosition) && evt.type == EventType.ContextClick)
            {
                GenericMenu menu = new GenericMenu();

                AddUpgradeAndDowngradeOptions(menu, property);

                menu.ShowAsContext();

                evt.Use();
            }

            NextHeight();
        }

        EditorGUI.indentLevel--;
    }

    private void OnPropertyContextMenu(GenericMenu menu, SerializedProperty property)
    {
        if (property.propertyType == SerializedPropertyType.Float)
        {
            int lastDotIndex = property.propertyPath.LastIndexOf('.');

            if (lastDotIndex < 0)
            {
                return;
            }

            string fixedPath = property.propertyPath.Substring(0, lastDotIndex);

            SerializedProperty fixedProp = property.serializedObject.FindProperty(fixedPath);
            if (fixedProp.type == "UpgradeableFloat")
            {
                AddUpgradeAndDowngradeOptions(menu, fixedProp);
            }
        }

        switch (property.type)
        {
            case "UpgradeableFloat":
                AddUpgradeAndDowngradeOptions(menu, property);
                break;

            case "Tier":
                int lastDotIndex = property.propertyPath.LastIndexOf('.');

                if (lastDotIndex < 0)
                {
                    return;
                }

                string fixedPath = property.propertyPath.Substring(0, lastDotIndex);

                if (!fixedPath.EndsWith(".Array"))
                {
                    return;
                }

                fixedPath = fixedPath.Substring(0, fixedPath.Length - 6);

                bool isDowngrade = fixedPath.Substring(fixedPath.LastIndexOf('.') + 1).Trim() == "downgrades";

                string tierType = isDowngrade ? "Downgrade" : "Upgrade";

                Action<object> callback = (obj) =>
                {
                    GUIContent content = (GUIContent)_genericMenuItemContentField.GetValue(obj);

                    if (content.text.StartsWith("Duplicate"))
                    {
                        content.text = "Duplicate " + tierType;
                    }
                    else if (content.text.StartsWith("Delete"))
                    {
                        content.text = "Delete " + tierType;
                    }
                };

                object list = _genericMenuItemsField.GetValue(menu);
                _loopThroughAllMenuItemsInList.Invoke(null, new object[] { list, callback });

                SerializedProperty array = property.serializedObject.FindProperty(fixedPath);
                int arraySize = array.arraySize;

                string indexString = property.propertyPath.Substring(property.propertyPath.LastIndexOf('[') + 1);
                indexString = indexString.Substring(0, indexString.Length - 1);
                if (!int.TryParse(indexString, out int index))
                {
                    index = -1;
                }

                bool lowerBound = index <= 0;
                bool upperBound = index < 0 || index >= arraySize - 1;

                void SwapWith(int otherIndex)
                {
                    SerializedProperty otherProp = array.GetArrayElementAtIndex(otherIndex);

                    SerializedProperty valueProp = property.FindPropertyRelative("value");
                    SerializedProperty otherValueProp = otherProp.FindPropertyRelative("value");

                    float tempValue = otherValueProp.floatValue;
                    otherValueProp.floatValue = valueProp.floatValue;
                    valueProp.floatValue = tempValue;

                    SerializedProperty methodProp = property.FindPropertyRelative("method");
                    SerializedProperty otherMethodProp = otherProp.FindPropertyRelative("method");

                    int tempMethod = otherMethodProp.enumValueIndex;
                    otherMethodProp.enumValueIndex = methodProp.enumValueIndex;
                    methodProp.enumValueIndex = tempMethod;

                    property.serializedObject.ApplyModifiedProperties();
                }

                menu.AddItem(new GUIContent("Move up"), false, (isDowngrade ? lowerBound : upperBound ) ? null : () =>
                {
                    SwapWith(index + (isDowngrade ? -1 : 1));
                });

                menu.AddItem(new GUIContent("Move down"), false, (isDowngrade ? upperBound : lowerBound) ? null : () =>
                {
                    SwapWith(index + (isDowngrade ? 1 : -1));
                });

                menu.AddSeparator("");

                AddMethodOptions(menu, "Calculation Method/", property.FindPropertyRelative("method"));
                
                break;
        }
    }

    private static void LoopThroughAllMenuItemsInList<T>(List<T> list, Action<object> callback)
    {
        foreach (T item in list)
        {
            callback?.Invoke(item);
        }
    }

    private static void AddUpgradeAndDowngradeOptions(GenericMenu menu, SerializedProperty property)
    {
        void AddTo(string propertyName)
        {
            SerializedProperty prop = property.FindPropertyRelative(propertyName);

            int index = prop.arraySize;
            prop.InsertArrayElementAtIndex(index);
            //upgradesProp.GetArrayElementAtIndex(index);

            property.serializedObject.ApplyModifiedProperties();
        }

        menu.AddItem(new GUIContent("Add Upgrade"), false, () =>
        {
            AddTo("upgrades");
        });
        menu.AddItem(new GUIContent("Add Downgrade"), false, () =>
        {
            AddTo("downgrades");
        });
    }

    private static void AddMethodOptions(GenericMenu menu, string folder, SerializedProperty property)
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

    private static void DrawTier(Rect rect, SerializedProperty property, GUIContent label, bool isDowngrade)
    {
        label = EditorGUI.BeginProperty(rect, label, property);

        SerializedProperty valueProp = property.FindPropertyRelative("value");
        SerializedProperty methodProp = property.FindPropertyRelative("method");

        Rect prefixRect = GetPrefixRect(rect);

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
        /*
        else if (prefixRect.Contains(evt.mousePosition) && evt.type == EventType.ContextClick)
        {
            evt.Use();
        }
        */

        valueProp.floatValue = EditorGUI.FloatField(rect, label, valueProp.floatValue);

        UpgradeableFloat.Method method = (UpgradeableFloat.Method)methodProp.enumValueIndex;

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

    private static Rect GetPrefixRect(Rect rect) => new Rect(rect.x + EditorGUI.indentLevel * 15, rect.y, EditorGUIUtility.labelWidth - EditorGUI.indentLevel * 15, rect.height);

    private static void DoFadedLabel(Rect rect, GUIContent label, bool italic = true)
    {
        if (_italicLabelStyle == null)
        {
            _italicLabelStyle = new GUIStyle(EditorStyles.label);

            _italicLabelStyle.fontStyle = FontStyle.Italic;
        }

        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUI.LabelField(rect, label, italic ? _italicLabelStyle : EditorStyles.label);
        }
    }
    private static void DoFadedLabel(Rect rect, string label, bool italic = true) => DoFadedLabel(rect, new GUIContent(label), italic);

    private static void DrawBGBox(Rect rect)
    {
        EditorGUI.DrawRect(rect, new Color(0, 0, 0, 0.1f));
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty baseProp = property.FindPropertyRelative("baseValue");

        float height = EditorGUIUtility.singleLineHeight;

        if (baseProp.isExpanded)
        {
            height += EditorGUIUtility.standardVerticalSpacing;

            height += SPACING * 4;

            SerializedProperty upgradesProp = property.FindPropertyRelative("upgrades");
            SerializedProperty downgradesProp = property.FindPropertyRelative("downgrades");

            int upgradesArraySize = upgradesProp.arraySize;
            int downgradesArraySize = downgradesProp.arraySize;

            int count = Mathf.Max(upgradesArraySize, 1) + Mathf.Max(downgradesArraySize, 1) + 1;

            height += EditorGUIUtility.singleLineHeight * (float)count + EditorGUIUtility.standardVerticalSpacing * (float)count;
        }

        return height;
    }
}
