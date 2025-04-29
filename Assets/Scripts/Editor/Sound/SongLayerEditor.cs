using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SongLayer))]
public class SongLayerEditor : Editor
{
    private SongLayer _target;

    private SerializedProperty _songProperty;

    private Song _parent;

    private void OnEnable()
    {
        _target = (SongLayer)target;

        _songProperty = serializedObject.FindProperty("song");

        Undo.undoRedoPerformed += OnUndoRedo;
    }

    private void OnDisable()
    {
        Undo.undoRedoPerformed -= OnUndoRedo;
    }

    private void OnUndoRedo()
    {
        TrySaveParent();
    }

    private void TrySaveParent()
    {
        if (_parent == null)
        {
            return;
        }

        CustomEditorUtility.SaveAsset(_parent);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        bool isSubAsset = AssetDatabase.IsSubAsset(_target);

        if (isSubAsset)
        {
            _parent = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(_target)) as Song;

            if (_parent != null)
            {
                string oldName = _target.name;
                string newName = EditorGUILayout.DelayedTextField("Name", oldName);

                if (oldName != newName)
                {
                    Undo.RegisterCompleteObjectUndo(_target, "Renamed " + oldName + " to " + newName);
                    _target.name = newName;

                    TrySaveParent();
                }

                if (GUILayout.Button("Remove from song"))
                {
                    Undo.DestroyObjectImmediate(_target);
                    Undo.SetCurrentGroupName("Deleted " + newName + " from " + _parent.name);

                    TrySaveParent();
                    return;
                }

                EditorGUILayout.Space(12);
            }
        }
        else
        {
            _parent = null;
        }

        _songProperty.objectReferenceValue = _parent;

        SerializedProperty iterator = serializedObject.GetIterator();
        bool enterChildren = true;

        while (iterator.NextVisible(enterChildren))
        {
            string propertyPath = iterator.propertyPath;

            switch (propertyPath)
            {
                case "m_Script":
                case "song":
                    continue;
            }

            EditorGUILayout.PropertyField(iterator, true);

            enterChildren = false;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
