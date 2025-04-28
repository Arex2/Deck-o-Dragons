using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(Song))]
public class SongEditor : Editor
{
    private Song _target;

    private SerializedProperty _layersProp;
    private SerializedProperty _stylesProp;

    private ReorderableList _list;

    private bool _isSubAsset;
    private Object[] _allSubAssets;

    private void OnEnable()
    {
        _target = (Song)target;

        _layersProp = serializedObject.FindProperty("layers");
        _stylesProp = serializedObject.FindProperty("styles");

        _list = new ReorderableList(serializedObject, _stylesProp);

        _list.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Styles", EditorStyles.boldLabel);

        _list.drawElementCallback = DrawStyleElement;
        _list.elementHeightCallback = GetStyleElementHeight;

        CacheLayers();

        Undo.undoRedoPerformed += OnUndoRedo;
    }

    private void OnDisable()
    {
        Undo.undoRedoPerformed -= OnUndoRedo;
    }

    private void OnUndoRedo()
    {
        SaveAsset();

        CacheLayers();
    }

    private void SaveAsset()
    {
        CustomEditorUtility.SaveAsset(target);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        _isSubAsset = AssetDatabase.IsSubAsset(_target);
        _allSubAssets = _isSubAsset ? null : AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(_target));

        SerializedProperty iterator = serializedObject.GetIterator();
        bool enterChildren = true;
        while (iterator.NextVisible(enterChildren))
        {
            string propertyPath = iterator.propertyPath;

            bool disable;

            switch (propertyPath)
            {
                case "m_Script":
                case "styles":
                    continue;

                case "layers":
                    disable = !_isSubAsset;
                    break;

                default:
                    disable = false;
                    break;
            }

            using (new EditorGUI.DisabledScope(disable))
            {
                EditorGUILayout.PropertyField(iterator, true);
            }

            if (propertyPath == "layers")
            {
                using (new EditorGUI.DisabledScope(_isSubAsset))
                {
                    if (GUILayout.Button("Add new layer"))
                    {
                        ScriptableObject newLayer = CreateInstance<SongLayer>();

                        newLayer.name = "New Layer";

                        AssetDatabase.AddObjectToAsset(newLayer, _target);

                        string undoName = $"Added " + newLayer.name + " to " + _target.name;
                        Undo.RegisterCreatedObjectUndo(newLayer, undoName);

                        Undo.SetCurrentGroupName(undoName);

                        SaveAsset();

                        CacheLayers();

                        Selection.objects = new Object[] { newLayer };
                        EditorGUIUtility.PingObject(newLayer);
                    }
                }
            }

            enterChildren = false;
        }

        EditorGUILayout.Space();
        _list.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }

    private void CacheLayers()
    {
        if (AssetDatabase.IsSubAsset(_target))
        {
            return;
        }

        int index = 0;

        _layersProp.ClearArray();

        foreach (Object obj in AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(_target)))
        {
            SongLayer layer = obj as SongLayer;

            if (layer == null)
            {
                continue;
            }

            using (SerializedObject tempObj = new SerializedObject(layer))
            {
                tempObj.FindProperty("song").objectReferenceValue = _target;

                tempObj.ApplyModifiedProperties();
            }

            _layersProp.InsertArrayElementAtIndex(index);
            _layersProp.GetArrayElementAtIndex(index).objectReferenceValue = layer;
            index++;
        }
    }

    private void DrawStyleElement(Rect totalRect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty property = _stylesProp.GetArrayElementAtIndex(index);

        Rect rect = totalRect;
        rect.height = EditorGUIUtility.singleLineHeight;

        void NextHeight(float? height = null)
        {
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;

            rect.height = height.HasValue ? height.Value : EditorGUIUtility.singleLineHeight;
        }

        Rect foldoutProp = rect;
        foldoutProp.xMin += 20;

        SerializedProperty nameProp = property.FindPropertyRelative("name");

        EditorGUI.PropertyField(foldoutProp, nameProp, new GUIContent("Style " + index));

        Rect prefixRect = CustomEditorUtility.GetPrefixRect(foldoutProp);
        property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(prefixRect, property.isExpanded, GUIContent.none, EditorStyles.foldout);
        EditorGUI.EndFoldoutHeaderGroup();

        if (!property.isExpanded)
        {
            return;
        }

        NextHeight();

        rect.y += CustomEditorUtility.SPACING / 2;

        float yStart = rect.y;

        rect.y += CustomEditorUtility.SPACING;

        rect.xMin += CustomEditorUtility.SPACING / 2;
        rect.xMax -= CustomEditorUtility.SPACING / 2;

        SerializedProperty layersProp = property.FindPropertyRelative("layers");
        int arraySize = layersProp.arraySize;

        /*
        HashSet<SongLayer> layers = new();

        if (arraySize > 0)
        {
            for (int i = 0; i < arraySize; i++)
            {
                SongLayer layer = layersProp.GetArrayElementAtIndex(i).objectReferenceValue as SongLayer;

                if (layer == null || layers.Contains(layer))
                {
                    continue;
                }

                layers.Add(layer);
            }
        }

        if (!_isSubAsset)
        {
            foreach (Object obj in _allSubAssets)
            {
                if (obj == null || !(obj is SongLayer))
                {
                    continue;
                }

                SongLayer layer = obj as SongLayer;

                NextHeight();
            }
        }
        */

        if (arraySize > 0)
        {
            List<int> indicesToRemove = new();

            for (int i = 0; i < arraySize; i++)
            {
                SerializedProperty layerProp = layersProp.GetArrayElementAtIndex(i);

                Rect propertyRect = rect;
                Rect deleteButtonRect = rect;

                deleteButtonRect.xMin = deleteButtonRect.xMax - 20;
                propertyRect.xMax -= deleteButtonRect.width + CustomEditorUtility.SPACING / 2;

                if (_isSubAsset)
                {
                    EditorGUI.PropertyField(propertyRect, layerProp, GUIContent.none);
                }
                else
                {
                    Event evt = Event.current;

                    if (propertyRect.Contains(evt.mousePosition) && evt.type == EventType.MouseDown && evt.button == 0)
                    {
                        evt.Use();

                        GenericMenu menu = new();

                        foreach (Object obj in _allSubAssets)
                        {
                            if (obj == null || !(obj is SongLayer))
                            {
                                continue;
                            }

                            int currentStyleIndex = index;
                            int currentLayerIndex = i;
                            menu.AddItem(new(obj.name), layerProp.objectReferenceValue == obj, () =>
                            {
                                using (SerializedObject serializedObj = new(_target))
                                {
                                    serializedObj.FindProperty("styles").GetArrayElementAtIndex(currentStyleIndex).FindPropertyRelative("layers").GetArrayElementAtIndex(currentLayerIndex).objectReferenceValue = obj;
                                    serializedObj.ApplyModifiedProperties();
                                }
                            });
                        }

                        menu.DropDown(propertyRect);
                    }

                    GUIContent content;

                    if (layerProp.objectReferenceValue == null)
                    {
                        content = new("None");
                    }
                    else
                    {
                        content = EditorGUIUtility.ObjectContent(layerProp.objectReferenceValue, typeof(SongLayer));
                    }

                    EditorGUI.DropdownButton(propertyRect, content, FocusType.Keyboard);

                    EditorGUI.BeginProperty(propertyRect, GUIContent.none, layerProp);
                    EditorGUI.EndProperty();
                }

                if (GUI.Button(deleteButtonRect, EditorGUIUtility.IconContent("P4_DeletedLocal"), EditorStyles.miniButtonMid))
                {
                    indicesToRemove.Add(i);
                }

                NextHeight();
            }

            for (int i = indicesToRemove.Count - 1; i >= 0; i--)
            {
                layersProp.DeleteArrayElementAtIndex(indicesToRemove[i]);
                arraySize--;
            }
        }
        else
        {
            CustomEditorUtility.DoFadedLabel(rect, "No layers");
            NextHeight();
        }

        Rect buttonRect = rect;

        if (GUI.Button(buttonRect, "Add"))
        {
            layersProp.InsertArrayElementAtIndex(arraySize);
        }

        NextHeight();

        rect.y += CustomEditorUtility.SPACING / 2;

        float yEnd = rect.y;

        Rect bgRect = totalRect;
        bgRect.yMin = yStart;
        bgRect.yMax = yEnd;

        CustomEditorUtility.DrawBGBox(bgRect);
    }

    private float GetStyleElementHeight(int index)
    {
        SerializedProperty property = _stylesProp.GetArrayElementAtIndex(index);

        int count = 1;
        float addedHeight = 0;

        if (property.isExpanded)
        {
            /*
            if (!_isSubAsset)
            {
                foreach (Object obj in _allSubAssets)
                {
                    if (obj == null || !(obj is SongLayer))
                    {
                        continue;
                    }

                    count++;
                }
            }
            */
            count += 1;

            SerializedProperty layersProp = property.FindPropertyRelative("layers");
            count += Mathf.Max(1, layersProp.arraySize);

            addedHeight += CustomEditorUtility.SPACING * 2;
        }

        return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * (float)count + addedHeight;
    }
}