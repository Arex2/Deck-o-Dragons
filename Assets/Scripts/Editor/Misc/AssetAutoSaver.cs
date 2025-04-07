using System;
using UnityEditor;
using System.Reflection;

/// <summary>
/// Forces Unity to perform the "Save Project" action whenever the editor gains or loses focus. <para/>
/// The "Save Project" action will write all Assets in the project to the disk. <para/>
/// Made so that Github won't miss certain objects being changed.
/// </summary>
// Script by Ruben
public static class AssetAutoSaver
{
    /// <summary>
    /// An event that is fired whenever the focus of the <see cref="EditorApplication"/> is changed. <para/>
    /// Code stolen from: https://discussions.unity.com/t/a-way-to-detect-when-the-editor-application-is-focused/174507/9
    /// </summary>
    public static Action<bool> UnityEditorFocusChanged
    {
        get
        {
            return (Action<bool>)_focusChangedField.GetValue(null);
        }
        set
        {
            _focusChangedField.SetValue(null, value);
        }
    }
    private static FieldInfo _focusChangedField = typeof(EditorApplication).GetField("focusChanged", BindingFlags.Static | BindingFlags.NonPublic);

    [InitializeOnLoadMethod]
    public static void Init()
    {
        // Force save on focus changed
        UnityEditorFocusChanged += (_) => ForceSaveProject();

        // Ignore first bootup by accesing a SessionState boolean
        if (!SessionState.GetBool("AssetAutoSaver.HasBooted", false))
        {
            // Set the SessionState boolean to true
            SessionState.SetBool("AssetAutoSaver.HasBooted", true);

            // The reason we do this is because the menu items haven't initialized yet
            // If the menu items don't exist, then an error occurs!

            return;
        }

        ForceSaveProject();
    }

    private static void ForceSaveProject()
    {
        // Force unity to save the project
        EditorApplication.ExecuteMenuItem("File/Save Project");
    }
}