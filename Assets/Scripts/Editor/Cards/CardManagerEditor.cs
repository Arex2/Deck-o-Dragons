using UnityEditor;

/// <summary>
/// The custom editor for <see cref="CardManager"/>.
/// </summary>
// Script by Ruben
[CustomEditor(typeof(CardManager))]
public class CardManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();

        EditorGUI.BeginDisabledGroup(true);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("allCards"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("allTags"));

        EditorGUI.EndDisabledGroup();
    }
}
