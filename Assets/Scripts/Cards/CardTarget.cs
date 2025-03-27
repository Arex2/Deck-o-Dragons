/// <summary>
/// Enum that represents the different kinds of targets a <see cref="Card"/> can be used on.
/// </summary>
// Script by Ruben
public enum CardTarget
{
    /// <summary>
    /// The player gets to choose what to target before the card is used.
    /// </summary>
    Chosen,
    /// <summary>
    /// The enemies are targeted.
    /// </summary>
    Enemy,
    /// <summary>
    /// The player is targeted.
    /// </summary>
    Self,
}
