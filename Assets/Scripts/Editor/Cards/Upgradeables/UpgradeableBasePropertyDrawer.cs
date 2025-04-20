using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// The base property drawer script for every Upgradeable property drawer like <see cref="UpgradeablePropertyDrawer"/> and <see cref="UpgradeableNumberPropertyDrawer{T}"/>.
/// </summary>
// Script by Ruben
public abstract class UpgradeableBasePropertyDrawer<T> : PropertyDrawer
{
    private const string STACK_LOOP_SESSION_STATE_NAME = "ShowStackAndLoopState";
    private static bool _showStackAndLoopState => SessionState.GetBool(STACK_LOOP_SESSION_STATE_NAME, false);

    private static readonly FieldInfo _genericMenuItemsField = typeof(GenericMenu).GetField("m_MenuItems", BindingFlags.NonPublic | BindingFlags.Instance);
    private static readonly Type _genericMenuItemType = typeof(GenericMenu).GetNestedType("MenuItem", BindingFlags.NonPublic);
    private static readonly FieldInfo _genericMenuItemContentField = _genericMenuItemType.GetField("content", BindingFlags.Public | BindingFlags.Instance);
    private static readonly MethodInfo _loopThroughAllMenuItemsInList = typeof(UpgradeableBasePropertyDrawer<T>).GetMethod(nameof(LoopThroughAllMenuItemsInList), BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(_genericMenuItemType);

    private T _oldValue;
    private bool _shouldExpand;

    public UpgradeableBasePropertyDrawer() : base()
    {
        EditorApplication.contextualPropertyMenu += OnPropertyContextMenu;
    }

    ~UpgradeableBasePropertyDrawer()
    {
        EditorApplication.contextualPropertyMenu -= OnPropertyContextMenu;
    }

    public abstract bool CanStack { get; }

    public abstract bool MatchesThisClass(string propertyType);
    public abstract bool MatchesTier(string propertyType);

    public abstract T GetPropValue(SerializedProperty prop);

    public abstract float GetPropHeight(SerializedProperty prop);

    public abstract float GetTierHeight(SerializedProperty prop);

    public abstract void DrawProp(Rect rect, SerializedProperty prop, GUIContent label);

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty baseProp = property.FindPropertyRelative("baseValue");

        Rect rect = position;
        rect.height = GetPropHeight(baseProp);

        Event evt = Event.current;

        void NextHeight(float? height = null)
        {
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;

            rect.height = height.HasValue ? height.Value : EditorGUIUtility.singleLineHeight;
        }

        GUIContent propertyLabel = EditorGUI.BeginProperty(rect, label, property);

        Rect foldoutPosition = CustomEditorUtility.GetPrefixRect(rect);

        if (evt.type == EventType.MouseDown && evt.button == 0)
        {
            _shouldExpand = true;

            _oldValue = GetPropValue(baseProp);
        }

        if (evt.type == EventType.MouseUp && evt.button == 0)
        {
            if (_shouldExpand && foldoutPosition.Contains(evt.mousePosition))
            {
                baseProp.isExpanded = !baseProp.isExpanded;
                evt.Use();
            }
        }

        EditorGUI.EndProperty();

        DrawProp(rect, baseProp, propertyLabel);

        T newValue = GetPropValue(baseProp);
        bool equals = (_oldValue == null && newValue == null) || (_oldValue != null && newValue != null && _oldValue.Equals(newValue));

        if (!equals)
        {
            _shouldExpand = false;
        }

        baseProp.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(foldoutPosition, baseProp.isExpanded, CustomEditorUtility.EmptyContent, EditorStyles.foldout);
        EditorGUI.EndFoldoutHeaderGroup();

        if (!baseProp.isExpanded)
        {
            return;
        }

        Rect remainderRect = position;
        remainderRect.yMin += rect.height + CustomEditorUtility.SPACING / 2;

        CustomEditorUtility.DrawBGBox(remainderRect);

        rect.y += CustomEditorUtility.SPACING;

        EditorGUI.indentLevel++;

        NextHeight();

        SerializedProperty stackProp = property.FindPropertyRelative("stack");

        if (_showStackAndLoopState)
        {
            if (CanStack)
            {
                EditorGUI.PropertyField(rect, stackProp);

                NextHeight();
            }
            else
            {
                stackProp.boolValue = false;
            }

            SerializedProperty loopBehaviourProp = property.FindPropertyRelative("loopBehaviour");

            EditorGUI.PropertyField(rect, loopBehaviourProp);

            NextHeight();

            rect.y += CustomEditorUtility.SPACING;
        }

        bool stack = stackProp.boolValue;

        // Upgrades
        SerializedProperty upgradesProp = property.FindPropertyRelative("upgrades");
        int upgradesArraySize = upgradesProp.arraySize;

        if (upgradesArraySize > 0)
        {
            T result = GetPropValue(baseProp);

            Rect[] rects = new Rect[upgradesArraySize];

            for (int i = upgradesArraySize - 1; i >= 0; i--)
            {
                rect.height = GetTierHeight(upgradesProp.GetArrayElementAtIndex(i));

                rects[i] = rect;

                NextHeight();
            }

            for (int i = 0; i < upgradesArraySize; i++)
            {
                if (!stack)
                {
                    result = GetPropValue(baseProp);
                }

                DrawTier(rects[i], upgradesProp.GetArrayElementAtIndex(i), new GUIContent("Upgrade " + (i + 1)), false, ref result);
            }
        }
        else
        {
            CustomEditorUtility.DoFadedLabel(rect, "This field has no upgrades. Right click to add some!");

            if (rect.Contains(evt.mousePosition) && evt.type == EventType.ContextClick)
            {
                GenericMenu menu = new GenericMenu();

                AddUpgradeAndDowngradeOptions(menu, property);

                menu.ShowAsContext();

                evt.Use();
            }

            NextHeight();
        }

        rect.y += CustomEditorUtility.SPACING;

        rect.height = GetPropHeight(baseProp);

        Rect bgRect = rect;

        bgRect.height += CustomEditorUtility.SPACING;
        bgRect.y -= CustomEditorUtility.SPACING / 2;

        CustomEditorUtility.DrawBGBox(bgRect);

        propertyLabel = EditorGUI.BeginProperty(rect, new GUIContent("Base Value"), baseProp);
        EditorGUI.EndProperty();

        DrawProp(rect, baseProp, propertyLabel);

        NextHeight();

        rect.y += CustomEditorUtility.SPACING;

        // Downgrades
        SerializedProperty downgradesProp = property.FindPropertyRelative("downgrades");
        int downgradesArraySize = downgradesProp.arraySize;

        if (downgradesArraySize > 0)
        {
            T result = GetPropValue(baseProp);

            for (int i = 0; i < downgradesArraySize; i++)
            {
                if (!stack)
                {
                    result = GetPropValue(baseProp);
                }

                SerializedProperty arrayElement = downgradesProp.GetArrayElementAtIndex(i);

                rect.height = GetTierHeight(arrayElement);

                DrawTier(rect, arrayElement, new GUIContent("Downgrade " + (i + 1)), true, ref result);

                NextHeight();
            }
        }
        else
        {
            CustomEditorUtility.DoFadedLabel(rect, "This field has no downgrades. Right click to add some!");

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
        int lastDotIndex;
        string fixedPath;

        if (MatchesThisClass(property.type))
        {
            AddUpgradeAndDowngradeOptions(menu, property);

            menu.AddSeparator("");

            AddStackAndLoopOptions(menu);
            return;
        }
        else if (MatchesTier(property.type))
        {
            lastDotIndex = property.propertyPath.LastIndexOf('.');

            if (lastDotIndex < 0)
            {
                return;
            }

            fixedPath = property.propertyPath.Substring(0, lastDotIndex);

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

            menu.AddItem(new GUIContent("Move up"), false, (isDowngrade ? lowerBound : upperBound) ? null : () =>
            {
                SwapWith(index + (isDowngrade ? -1 : 1));
            });

            menu.AddItem(new GUIContent("Move down"), false, (isDowngrade ? upperBound : lowerBound) ? null : () =>
            {
                SwapWith(index + (isDowngrade ? 1 : -1));
            });

            menu.AddSeparator("");

            AddStackAndLoopOptions(menu);
            AddMoreMenuOptions(menu, property);

            return;
        }

        lastDotIndex = property.propertyPath.LastIndexOf('.');

        if (lastDotIndex < 0)
        {
            return;
        }

        fixedPath = property.propertyPath.Substring(0, lastDotIndex);

        SerializedProperty fixedProp = property.serializedObject.FindProperty(fixedPath);
        if (fixedProp.type != "UpgradeableFloat")
        {
            return;
        }

        AddUpgradeAndDowngradeOptions(menu, fixedProp);

        menu.AddSeparator("");

        AddStackAndLoopOptions(menu);
    }

    private static void LoopThroughAllMenuItemsInList<T2>(List<T2> list, Action<object> callback)
    {
        foreach (T2 item in list)
        {
            callback?.Invoke(item);
        }
    }

    // TODO: Add a clean list method

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

    private static void AddStackAndLoopOptions(GenericMenu menu)
    {
        bool currentShowStackAndLoopState = _showStackAndLoopState;

        menu.AddItem(new GUIContent("Show Stack and Loop options"), currentShowStackAndLoopState, () =>
        {
            SessionState.SetBool(STACK_LOOP_SESSION_STATE_NAME, !currentShowStackAndLoopState);
        });
    }

    public abstract void AddMoreMenuOptions(GenericMenu menu, SerializedProperty property);


    public abstract void DrawTier(Rect rect, SerializedProperty property, GUIContent label, bool isDowngrade, ref T result);

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty baseProp = property.FindPropertyRelative("baseValue");

        float height = GetPropHeight(baseProp);

        if (baseProp.isExpanded)
        {
            height += EditorGUIUtility.standardVerticalSpacing * 2;
            height += GetPropHeight(baseProp);

            height += CustomEditorUtility.SPACING * 4;

            if (_showStackAndLoopState)
            {
                height += CustomEditorUtility.SPACING;
            }

            int count = 0;

            SerializedProperty upgradesProp = property.FindPropertyRelative("upgrades");
            int upgradesArraySize = upgradesProp.arraySize;

            if (upgradesArraySize <= 0)
            {
                count++;
            }
            else
            {
                for (int i = upgradesArraySize - 1; i >= 0; i--)
                {
                    height += GetTierHeight(upgradesProp.GetArrayElementAtIndex(i));
                }
            }

            SerializedProperty downgradesProp = property.FindPropertyRelative("downgrades");
            int downgradesArraySize = downgradesProp.arraySize;

            if (downgradesArraySize <= 0)
            {
                count++;
            }
            else
            {
                for (int i = 0; i < downgradesArraySize; i++)
                {
                    height += GetTierHeight(downgradesProp.GetArrayElementAtIndex(i));
                }
            }

            if (_showStackAndLoopState)
            {
                count += CanStack ? 2 : 1;
            }

            height += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * (float)count;
        }

        return height;
    }
}
