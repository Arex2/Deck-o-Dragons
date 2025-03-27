using System;

/// <summary>
/// An enum used to determine how the <see cref="CacheComponentAttribute"/> will cache it's <see cref="UnityEngine.Component"/>.
/// </summary>
[Flags]
public enum CacheMethod
{
    /// <summary>
    /// Does GetComponent.
    /// </summary>
    Default = 1,
    /// <summary>
    /// Does GetComponentInChildren.
    /// </summary>
    InChildren = 2,
    /// <summary>
    /// Does GetComponentInParent.
    /// </summary>
    InParent = 4,
    /// <summary>
    /// Does GetComponentInChildren. If that fails it does GetComponentInParent.
    /// </summary>
    All = Default | InChildren | InParent,
}