using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using Object = UnityEngine.Object;

[CustomPropertyDrawer(typeof(CardVFXTagAttribute))]
public class CardVFXTagAttributePropertyDrawer : PropertyDrawer
{
    public const string NO_TAG = "<no_tag>";

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.HelpBox(position, "The CardVFXTag attribute cannot be used on fields that aren't Strings!", MessageType.Error);
            return;
        }

        bool noLabel = label == GUIContent.none;
        GUIContent newLabel = EditorGUI.BeginProperty(position, label, property);

        Rect buttonRect;

        if (noLabel)
        {
            buttonRect = position;
        }
        else
        {
            buttonRect = EditorGUI.PrefixLabel(position, newLabel);
        }

        string value = property.stringValue;

        if (EditorGUI.DropdownButton(buttonRect, new GUIContent(string.IsNullOrEmpty(value) ? NO_TAG : value), FocusType.Keyboard))
        {
            Object target = property.serializedObject.targetObject;
            string propertyPath = property.propertyPath;

            SerializedObject serializedObject = CardVFXManagerEditor.Instance;

            serializedObject.UpdateIfRequiredOrScript();

            SerializedProperty tagsProp = serializedObject.FindProperty("tags");

            GenericMenu menu = new GenericMenu();

            if ((attribute as CardVFXTagAttribute).ShowNoTagOption)
            {
                menu.AddItem(new GUIContent(NO_TAG), string.IsNullOrEmpty(value), () =>
                {
                    using (SerializedObject obj = new SerializedObject(target))
                    {
                        obj.FindProperty(propertyPath).stringValue = string.Empty;
                        obj.ApplyModifiedProperties();
                    }
                });
                menu.AddSeparator("");
            }

            int arraySize = tagsProp.arraySize;
            for (int i = 0; i < arraySize; i++)
            {
                string tag = tagsProp.GetArrayElementAtIndex(i).stringValue;
                
                menu.AddItem(new GUIContent(tag), value == tag, () =>
                {
                    using (SerializedObject obj = new SerializedObject(target))
                    {
                        obj.FindProperty(propertyPath).stringValue = tag;
                        obj.ApplyModifiedProperties();
                    }
                });
            }

            menu.DropDown(buttonRect);
        }

        EditorGUI.EndProperty();
    }

    [InitializeOnLoadMethod]
    public static void InitContextualPropertyMenu()
    {
        Type type = null;

        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            type = assembly.GetType("UnityEditor.ScriptAttributeUtility");

            if (type != null)
            {
                break;
            }
        }

        if (type != null)
        {
            _getFieldInfoFromPropertyMethod = type.GetMethod("GetFieldInfoFromProperty", BindingFlags.NonPublic | BindingFlags.Static);
            EditorApplication.contextualPropertyMenu += OnContextualPropertyMenu;
        }
    }

    private static MethodInfo _getFieldInfoFromPropertyMethod = null;

    private static void OnContextualPropertyMenu(GenericMenu menu, SerializedProperty property)
    {
        if (property.propertyType != SerializedPropertyType.String)
        {
            return;
        }

        object[] parameters = new object[2]
        {
            property,
            default(Type),
        };

        FieldInfo fieldInfo = (FieldInfo)_getFieldInfoFromPropertyMethod.Invoke(null, parameters);

        if (fieldInfo == null)
        {
            return;
        }

        CardVFXTagAttribute attribute = fieldInfo.GetCustomAttribute<CardVFXTagAttribute>();

        if (attribute == null)
        {
            return;
        }

        menu.AddItem(new GUIContent("Add or remove VFX Tags"), false, CardVFXManagerEditor.OpenPropertyEditor);
    }
}
