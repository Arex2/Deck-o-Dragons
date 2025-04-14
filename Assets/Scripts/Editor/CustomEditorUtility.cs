using UnityEngine;
using UnityEditor;

/// <summary>
/// Static class that contains useful custom methods for making Editor tools.
/// </summary>
// Script by Ruben
public static class CustomEditorUtility
{
    private static GUIStyle _italicLabelStyle;

    public static Rect GetPrefixRect(Rect rect) => new Rect(rect.x + EditorGUI.indentLevel * 15, rect.y, EditorGUIUtility.labelWidth - EditorGUI.indentLevel * 15, rect.height);

    public static void DoFadedLabel(Rect rect, GUIContent label, bool italic = true)
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

    public static void DoFadedLabel(Rect rect, string label, bool italic = true) => DoFadedLabel(rect, new GUIContent(label), italic);

    public static void DrawBGBox(Rect rect)
    {
        EditorGUI.DrawRect(rect, new Color(0, 0, 0, 0.1f));
    }

}
