using UnityEngine;

/// <summary>
/// A <see cref="ScriptableObject"/> that saves its Globally Unique Identifier (GUID) to the <see cref="GUID"/> field.
/// </summary>
// Script by Ruben
public class GUIDScriptableObject : ScriptableObject
{
    /// <summary>
    /// The Globally Unique Identifier of this object.
    /// </summary>
    public string GUID => guid;
    [SerializeField] private string guid;
}
