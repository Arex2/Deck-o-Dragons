using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

/// <summary>
/// The custom editor script for <see cref="CardComponent"/>.
/// </summary>
// Script by Ruben
[CustomEditor(typeof(Card))]
public class CardEditor : Editor
{
    // I love regex
    private static readonly Regex _componentNameRegex = new Regex(@"^card[s]?(.+)", RegexOptions.IgnoreCase);
    private static readonly Regex _componentNumberIndexRegex = new Regex(@"(.*) [0-9]+");
    private static readonly string _componentResetRegexFormat = @"\""{0}\"":[^,]*,";

    private static readonly string[] _removedResetJsonValues = new string[]
    {
        "m_Enabled",
        "m_EditorHideFlags",
        "m_Name",
        "m_EditorClassIdentifier",
        "expandedInEditor",
        "orderInEditor",
        "card",
        "disableOnStart",
    };

    private CardComponentSearchWindowProvider _searchWindowProvider;

    private SerializedProperty _cardComponentsProperty;
    private bool _shouldCacheCardComponents = false;

    private Dictionary<Object, Editor> _editors = new();

    private Object _pendingRenameObj = null;
    private Object _currentRenameObj = null;

    private void OnEnable()
    {
        _cardComponentsProperty = serializedObject.FindProperty("cardComponents");

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

            name = newComponent.name;

            // Ensure the name is unique
            int arraySize = _cardComponentsProperty.arraySize;

            bool cardWithSameNameExists = false;
            int numberOfTries = 1;

            do
            {
                if (cardWithSameNameExists)
                {
                    Match numberIndexMatch = _componentNumberIndexRegex.Match(name);

                    name = (numberIndexMatch.Success ? numberIndexMatch.Result("$1").Trim() : name) + " " + numberOfTries;
                    newComponent.name = name;
                    numberOfTries++;
                }

                bool foundMatch = false;

                for (int i = 0; i < arraySize; i++)
                {
                    Object obj = _cardComponentsProperty.GetArrayElementAtIndex(i).objectReferenceValue;

                    if (obj == null)
                    {
                        continue;
                    }

                    if (obj.name == name)
                    {
                        foundMatch = true;
                        break;
                    }
                }

                cardWithSameNameExists = foundMatch;
            }
            while (cardWithSameNameExists);

            AssetDatabase.AddObjectToAsset(newComponent, target);

            using (SerializedObject serializedObject = new SerializedObject(newComponent))
            {
                serializedObject.FindProperty("orderInEditor").intValue = _cardComponentsProperty.arraySize;

                serializedObject.ApplyModifiedProperties();
            }

            string undoName = $"Added " + name + " to " + target.name;
            Undo.RegisterCreatedObjectUndo(newComponent, undoName);

            Undo.SetCurrentGroupName(undoName);

            SaveAsset();

            _shouldCacheCardComponents = true;
        };

        CacheCardComponents();

        Undo.undoRedoPerformed += OnUndoRedo;
    }

    private void OnDisable()
    {
        Undo.undoRedoPerformed -= OnUndoRedo;

        DestroyImmediate(_searchWindowProvider);
    }

    private void OnUndoRedo()
    {
        SaveAsset();

        _editors.Clear();
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

        DrawCardProperties();

        if (_shouldCacheCardComponents)
        {
            CacheCardComponents();

            _shouldCacheCardComponents = false;
        }

        DrawLine();

        bool shouldSaveAsset = false;
        Rect? renameRect = null;

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
            string foloutText = foldoutContent.text;
            foldoutContent.text = null;

            Rect rect = EditorGUILayout.GetControlRect();

            Rect enabledRect = rect;

            enabledRect.x += 22;
            enabledRect.width = 20;

            SerializedProperty disabledProp = editor.serializedObject.FindProperty("disableOnStart");
            string toggleName = "CARD_EDITOR:disableOnStart_toggle_" + i;

            Event evt = Event.current;

            if (enabledRect.Contains(evt.mousePosition) && evt.type == EventType.MouseDown && evt.button == 0)
            {
                disabledProp.boolValue = !disabledProp.boolValue;

                evt.Use();

                GUI.FocusControl(toggleName);
            }

            expandedProp.boolValue = EditorGUI.BeginFoldoutHeaderGroup(rect, expandedProp.boolValue, foldoutContent, null, (rect) =>
            {
                OpenCardComponentContextMenu(arrayObj, rect);
            });
            EditorGUILayout.EndFoldoutHeaderGroup();

            GUI.SetNextControlName(toggleName);
            EditorGUI.Toggle(enabledRect, !disabledProp.boolValue);

            rect.x += 39;
            rect.width -= 39;

            bool shouldRename = _pendingRenameObj == arrayObj || _currentRenameObj == arrayObj;

            if (!shouldRename)
            {
                EditorGUI.LabelField(rect, foloutText, EditorStyles.boldLabel);
            }
            else
            {
                renameRect = rect;

                _currentRenameObj = arrayObj;
            }

            editor.serializedObject.ApplyModifiedProperties();

            if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.ContextClick)
            {
                OpenCardComponentContextMenu(arrayObj, null);
            }
            
            if (expandedProp.boolValue)
            {
                DrawLine(false, 0.35f);

                if (editor is CardComponentEditor)
                {
                    (editor as CardComponentEditor).CardEditorInspector();
                }
                else
                {
                    editor.OnInspectorGUI();
                }

                DrawLine();
            }
            else
            {
                DrawLine(false);
            }
        }

        if (renameRect.HasValue)
        {
            GUI.SetNextControlName("RenameField");
            string oldName = _currentRenameObj.name;
            Undo.RecordObject(_currentRenameObj, "Rename " + oldName);
            _currentRenameObj.name = EditorGUI.DelayedTextField(renameRect.Value, oldName);

            Event evt = Event.current;

            if (_pendingRenameObj != null)
            {
                _pendingRenameObj = null;

                EditorGUI.FocusTextInControl("RenameField");
            }
            else if ((evt.type != EventType.Repaint && evt.type != EventType.Layout && GUI.GetNameOfFocusedControl() != "RenameField") || oldName != _currentRenameObj.name)
            {
                SaveAsset();
                //Repaint();

                _currentRenameObj = null;
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

        if (shouldSaveAsset)
        {
            SaveAsset();
        }
    }

    private void DrawCardProperties()
    {
        SerializedProperty iterator = serializedObject.GetIterator();

        bool enterChildren = true;

        while (iterator.NextVisible(enterChildren))
        {
            enterChildren = false;

            switch (iterator.propertyPath)
            {
                case "m_Script":
                    continue;

                case "guid":
                    GUIDScriptableObjectEditor.HandleGUIDField(iterator, target);
                    continue;

                case "description":
                    EditorGUILayout.LabelField(iterator.displayName);

                    iterator.stringValue = EditorGUILayout.TextArea(iterator.stringValue, GUILayout.Height(60));
                    continue;

                case "sprite":
                    iterator.objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent(iterator.displayName), iterator.objectReferenceValue, typeof(Sprite), false, GUILayout.Height(64));
                    continue;
            }

            EditorGUILayout.PropertyField(iterator, true);
        }
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

        int index = -1;
        int arraySize = _cardComponentsProperty.arraySize;

        Object GetArrayObj(int index) => _cardComponentsProperty.GetArrayElementAtIndex(index).objectReferenceValue;

        for (int i = 0; i < arraySize; i++)
        {
            if (GetArrayObj(i) != cardComponent)
            {
                continue;
            }

            index = i;
            break;
        }

        menu.AddItem(new GUIContent("Rename Component"), false, () =>
        {
            _pendingRenameObj = cardComponent;
        });

        menu.AddItem(new GUIContent("Delete Component"), false, () =>
        {
            _editors.Remove(cardComponent);

            string name = cardComponent.name;

            Undo.DestroyObjectImmediate(cardComponent);

            //AssetDatabase.RemoveObjectFromAsset(target);
            Undo.SetCurrentGroupName("Deleted " + name + " from " + target.name);

            SaveAsset();

            _shouldCacheCardComponents = true;
        });

        menu.AddSeparator("");

        // Swapping orders
        void SwapCardOrder(int indexA, int indexB)
        {
            Object objA = GetArrayObj(indexA);
            Object objB = GetArrayObj(indexB);
           
            using (SerializedObject serializedObjA = new SerializedObject(objA))
            {
                using (SerializedObject serializedObjB = new SerializedObject(objB))
                {
                    SerializedProperty propA = serializedObjA.FindProperty("orderInEditor");
                    SerializedProperty propB = serializedObjB.FindProperty("orderInEditor");

                    int temp = propA.intValue;
                    propA.intValue = propB.intValue;
                    propB.intValue = temp;

                    serializedObjA.ApplyModifiedProperties();
                    serializedObjB.ApplyModifiedProperties();
                }
            }

            _shouldCacheCardComponents = true;
        }

        menu.AddItem(new GUIContent("Move up"), false, index <= 0 ? null : () =>
        {
            SwapCardOrder(index, index - 1);
        });

        menu.AddItem(new GUIContent("Move down"), false, index < 0 || index >= arraySize - 1 ? null : () =>
        {
            SwapCardOrder(index, index + 1);
        });

        menu.AddSeparator("");

        /*
        menu.AddItem(new GUIContent("Select in inspector"), false, () =>
        {
            Selection.activeObject = cardComponent;
            EditorGUIUtility.PingObject(cardComponent);
        });
        */

        menu.AddItem(new GUIContent("Edit Script"), false, () =>
        {
            using (SerializedObject obj = new SerializedObject(cardComponent))
            {
                EditorUtility.OpenWithDefaultApp(AssetDatabase.GetAssetPath(obj.FindProperty("m_Script").objectReferenceValue));
            }
        });

        menu.AddItem(new GUIContent("Reset"), false, () =>
        {
            Undo.RecordObject(cardComponent, "Reset " + cardComponent.name);

            Object defaultInstance = CreateInstance(cardComponent.GetType());

            string json = EditorJsonUtility.ToJson(defaultInstance);

            foreach (string removedJsonValue in _removedResetJsonValues)
            {
                json = Regex.Replace(json, string.Format(_componentResetRegexFormat, removedJsonValue), "");
            }

            EditorJsonUtility.FromJsonOverwrite(json, cardComponent);
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

        List<(int, Object)> sortedList = new List<(int, Object)>();

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

            using (SerializedObject serializedObject = new SerializedObject(obj))
            {
                sortedList.Add((serializedObject.FindProperty("orderInEditor").intValue, obj));
            }
        }

        sortedList.Sort((a, b) => a.Item1.CompareTo(b.Item1));

        foreach (var pair in sortedList)
        {
            using (SerializedObject serializedObject = new SerializedObject(pair.Item2))
            {
                serializedObject.FindProperty("orderInEditor").intValue = index;
                serializedObject.ApplyModifiedProperties();
            }

            _cardComponentsProperty.InsertArrayElementAtIndex(index);
            _cardComponentsProperty.GetArrayElementAtIndex(index).objectReferenceValue = pair.Item2;

            index++;
        }

        serializedObject.ApplyModifiedProperties();
    }
}