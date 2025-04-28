using System;
using UnityEngine;

/// <summary>
/// <see cref="ScriptableObject"/> that gives cards different ways to be identified in groups.
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
    [NonSerialized]
    private string _cachedDisplayName;

    public string Description => description;
    public bool Hidden => hidden;
    public bool HasPotency => hasPotency;

    [SerializeField] private string displayName;

    [TextArea(1, 3)]
    [SerializeField] private string description;

    [Space]
    [SerializeField] private bool hidden;
    [SerializeField] private bool hasPotency;
}
