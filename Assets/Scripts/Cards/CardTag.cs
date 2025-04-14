using UnityEngine;

/// <summary>
/// Empty <see cref="ScriptableObject"/> used for the purpose of giving cards different ways to be identified in groups.
/// </summary>
// Script by Ruben
[CreateAssetMenu(menuName = "Cards/Create New Card Tag")]
public class CardTag : GUIDScriptableObject
{
    public string DisplayName => displayName;

    [SerializeField] private string displayName;
}
