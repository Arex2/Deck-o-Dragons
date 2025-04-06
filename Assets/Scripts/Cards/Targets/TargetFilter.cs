using System;
using UnityEngine;

/// <summary>
/// Class used to filter what <see cref="Target"/>s to, well... target.
/// </summary>
// Script by Ruben
[Serializable]
public class TargetFilter
{
    public FilterTeam Team => team;
    public FilterMode Mode => mode;

    [SerializeField] private FilterTeam team;
    [SerializeField] private FilterMode mode;

    public TargetFilter(FilterTeam team, FilterMode mode)
    {
        this.team = team;
        this.mode = mode;
    }

    /// <summary>
    /// An enum that determines what team a <see cref="TargetFilter"/> should target.
    /// </summary>
    public enum FilterTeam
    {
        Opponent,
        Own,
        Chosen,
        Random,
        All,
    }

    /// <summary>
    /// An enum that determines what specific target a <see cref="TargetFilter"/> should target.
    /// </summary>
    public enum FilterMode
    {
        Leader,
        Chosen,
        Random,
        All,
    }
}
