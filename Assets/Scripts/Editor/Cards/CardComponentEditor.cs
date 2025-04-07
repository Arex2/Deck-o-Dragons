using UnityEngine;
using UnityEditor;

/// <summary>
/// The custom editor script for <see cref="CardComponent"/>.
/// </summary>
// Script by Ruben
[CustomEditor(typeof(CardComponent), true)]
public class CardComponentEditor : Editor
{
    private SerializedProperty _cardProp;

    private void OnEnable()
    {
        if (target == null || serializedObject == null)
        {
            return;
        }

        _cardProp = serializedObject.FindProperty("card");
    }

    public override void OnInspectorGUI()
    {
        CardEditorInspector();
    }

    public virtual void CardEditorInspector()
    {
        if (target == null)
        {
            return;
        }

        bool isSubAsset = AssetDatabase.IsSubAsset(target);

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

        Object parentAsset = null;
        if (isSubAsset)
        {
            parentAsset = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(target));
            _cardProp.objectReferenceValue = parentAsset;
        }

        serializedObject.ApplyModifiedProperties();
    }
}