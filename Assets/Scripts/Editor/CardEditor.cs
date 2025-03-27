using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// The custom editor script for <see cref="CardComponent"/>.
/// </summary>
// Script by Ruben
[CustomEditor(typeof(Card))]
public class CardEditor : Editor
{
    private static readonly Regex _componentNameRegex = new Regex("(?i)card(.+)");

    private CardComponentSearchWindowProvider _searchWindowProvider;

    private SerializedProperty _cardComponentsProperty;
    private bool _shouldCacheCardComponents = false;

    private Dictionary<Object, Editor> _editors = new();

    private void OnEnable()
    {
        _searchWindowProvider = CreateInstance<CardComponentSearchWindowProvider>();

        _searchWindowProvider.OnSelectType = (type) =>
        {
            string name = type.Name;

            ScriptableObject newComponent = CreateInstance(type);

            Match match = _componentNameRegex.Match(name);

            if (match.Success)
            {
                newComponent.name = match.Result("$1").Trim();
            }
            else
            {
                newComponent.name = name;
            }

            AssetDatabase.AddObjectToAsset(newComponent, target);

            Undo.RegisterCreatedObjectUndo(newComponent, "Add " + name);

            SaveAsset();

            _shouldCacheCardComponents = true;
        };

        _cardComponentsProperty = serializedObject.FindProperty("cardComponents");

        CacheCardComponents();

        Undo.undoRedoPerformed += SaveAsset;
    }

    private void OnDisable()
    {
        DestroyImmediate(_searchWindowProvider);

        Undo.undoRedoPerformed -= SaveAsset;
    }

    private void SaveAsset()
    {
        SaveAsset(target);
    }

    public static void SaveAsset(Object asset)
    {
        EditorUtility.SetDirty(asset);
        AssetDatabase.SaveAssetIfDirty(asset);

        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(asset), ImportAssetOptions.ForceUpdate);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        SerializedProperty iterator = serializedObject.GetIterator();

        bool enterChildren = true;
        while (iterator.NextVisible(enterChildren))
        {
            switch (iterator.propertyPath)
            {
                case "m_Script":
                    continue;
            }

            EditorGUILayout.PropertyField(iterator, true);

            enterChildren = false;
        }

        /*
        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.PropertyField(_cardComponentsProperty);
        }
        */

        if (_shouldCacheCardComponents)
        {
            CacheCardComponents();

            _shouldCacheCardComponents = false;
        }

        DrawLine();

        int arraySize = _cardComponentsProperty.arraySize;
        for (int i = 0; i < arraySize; i++)
        {
            SerializedProperty arrayElement = _cardComponentsProperty.GetArrayElementAtIndex(i);
            Object arrayObj = arrayElement.objectReferenceValue;

            if (arrayObj == null)
            {
                continue;
            }

            EditorGUILayout.Space(-EditorGUIUtility.standardVerticalSpacing);

            _editors.TryGetValue(arrayObj, out Editor editor);

            CreateCachedEditor(arrayObj, null, ref editor);

            _editors[arrayObj] = editor;

            SerializedProperty expandedProp = editor.serializedObject.FindProperty("expandedInEditor");

            GUIContent foldoutContent = EditorGUIUtility.ObjectContent(arrayObj, arrayObj.GetType());

            Rect rect = EditorGUILayout.GetControlRect();

            expandedProp.boolValue = EditorGUI.BeginFoldoutHeaderGroup(rect, expandedProp.boolValue, foldoutContent, null, (rect) =>
            {
                OpenCardComponentContextMenu(arrayObj, rect);
            });
            EditorGUILayout.EndFoldoutHeaderGroup();
            editor.serializedObject.ApplyModifiedProperties();

            if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.ContextClick)
            {
                OpenCardComponentContextMenu(arrayObj, null);
            }
            
            if (expandedProp.boolValue)
            {
                DrawLine(false, 0.3f);

                editor.OnInspectorGUI();

                DrawLine();
            }
            else
            {
                DrawLine(false);
            }
        }

        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        Rect buttonRect = GUILayoutUtility.GetRect(240, 25);

        if (GUI.Button(buttonRect, "Add Card Component"))
        {
            Vector2 guiPoint = new Vector2(buttonRect.center.x, buttonRect.max.y + buttonRect.height / 2 + 10);
            Vector2 mousePoint = GUIUtility.GUIToScreenPoint(guiPoint);

            SearchWindow.Open(new SearchWindowContext(mousePoint), _searchWindowProvider);
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawLine(bool doSpacing = true, float alpha = 1)
    {
        if (doSpacing)
        {
            EditorGUILayout.Space();
        }

        Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(1));

        rect.x -= 30;
        rect.width += 60;

        EditorGUI.DrawRect(rect, new Color(0, 0, 0, alpha));
    }

    private void OpenCardComponentContextMenu(Object cardComponent, Rect? rect)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("Delete component"), false, () =>
        {
            string name = cardComponent.name;

            Undo.DestroyObjectImmediate(cardComponent);

            //AssetDatabase.RemoveObjectFromAsset(target);
            Undo.SetCurrentGroupName($"Delete \"{name}\"");

            SaveAsset();

            _shouldCacheCardComponents = true;
        });

        if (rect.HasValue)
        {
            menu.DropDown(rect.Value);
        }
        else
        {
            menu.ShowAsContext();
        }
    }

    private void CacheCardComponents()
    {
        _cardComponentsProperty.ClearArray();

        int index = 0;
        foreach (Object obj in AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(target)))
        {
            if (obj == null)
            {
                continue;
            }

            if (!typeof(CardComponent).IsAssignableFrom(obj.GetType()))
            {
                continue;
            }

            _cardComponentsProperty.InsertArrayElementAtIndex(index);
            _cardComponentsProperty.GetArrayElementAtIndex(index).objectReferenceValue = obj;

            index++;
        }

        serializedObject.ApplyModifiedProperties();
    }
}