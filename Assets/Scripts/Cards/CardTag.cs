using UnityEngine;

/// <summary>
/// Empty <see cref="ScriptableObject"/> used for the purpose of giving cards different ways to be identified in groups.
/// </summary>
// Script by Ruben
[CreateAssetMenu(menuName = "Cards/Create New Card Tag")]
public class CardTag : GUIDScriptableObject
{
    public string DisplayName
    {
        get
        {
            if (_cachedDisplayName == null)
            {
                _cachedDisplayName = string.IsNullOrEmpty(displayName) ? displayName : name;
            }

            return _cachedDisplayName;
        }
    }
    private string _cachedDisplayName;

    [SerializeField] private string displayName;
}
