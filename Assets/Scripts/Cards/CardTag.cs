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
    public string PotencyDescription => potencyDescription;
    public bool Hidden => hidden;
    public bool IsCurse => isCurse;
    public bool HasPotency => hasPotency;

    public string PotencyFormat => potencyFormat;
    public Optional<string> PotencyFormatSingle => potencyFormatSingle;

    [SerializeField] private string displayName;

    [TextArea(1, 3)]
    [SerializeField] private string description;
    [TextArea(1, 3)]
    [SerializeField] private string potencyDescription;

    [Space]
    [SerializeField] private bool hidden;
    [SerializeField] private bool isCurse;
    [SerializeField] private bool hasPotency;

    [Space]
    [SerializeField] private string potencyFormat = "{0}";
    [SerializeField] private Optional<string> potencyFormatSingle = new(false, "{0}");
}
