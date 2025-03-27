using System;

/// <summary>
/// Enum that represents the different ways a card can be discarded.
/// </summary>
// Script by Ruben
[Flags]
public enum CardDiscardMethod
{
    EndOfTurn = 1,
    Sacrifice = 2,
    Eaten = 4,
}