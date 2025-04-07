using System;

/// <summary>
/// Enum that represents the element a <see cref="Card"/> or Dragon can have.
/// </summary>
// Script by Ruben
[Flags]
public enum Element
{
    None = 0,
    Fire = 1,
    Water = 2,
    Wind = 4,
    Earth = 8,
}