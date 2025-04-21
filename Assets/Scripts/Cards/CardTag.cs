using UnityEngine;

/// <summary>
/// Empty <see cref="ScriptableObject"/> used for the purpose of giving cards different ways to be identified in groups.
/// </summary>
// Script by Ruben
[CreateAssetMenu(fileName = "CardTag", menuName = "Cards/Create New Card Tag", order = 20)]
public class CardTag : GUIDScriptableObject
{
    public string DisplayName
    {
        get
        {
            if (string.IsNullOrEmpty(_cachedDisplayName))
            {
                _cachedDisplayName = string.IsNullOrEmpty(displayName) ? name : displayName;
            }

            return _cachedDisplayName;
        }
    }
    private string _cachedDisplayName;

    public bool Hidden => hidden;

    [SerializeField] private string displayName;
    [SerializeField] private bool hidden;

    private void OnEnable()
    {
        _cachedDisplayName = null;
    }
}
