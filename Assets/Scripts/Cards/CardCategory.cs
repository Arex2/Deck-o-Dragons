using System;

/// <summary>
/// Enum that represents what kind of category a <see cref="Card"/> belongs to.
/// </summary>
// Script by Ruben
[Flags]
public enum CardCategory
{
    None = 0,
    Offense = 1,
    Defense = 2,
    Utility = 4,
}