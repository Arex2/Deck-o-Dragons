using System;
using Random = UnityEngine.Random;

/// <summary>
/// Enum that represents the different two teams a <see cref="Target"/> can be on.
/// </summary>
// Script by Ruben
public enum Team
{
    Player,
    Enemy,
}

/// <summary>
/// Contains useful methods for the <see cref="Team"/> enum.
/// </summary>
public static class Teams
{
    public static readonly Team[] AllTeams = (Team[])Enum.GetValues(typeof(Team));
    public static readonly int AllTeamsLength = AllTeams.Length;

    /// <summary>
    /// Returns the opponent <see cref="Team"/> of this <see cref="Team"/>.
    /// </summary>
    public static Team GetOpponentTeam(this Team team)
    {
        switch (team)
        {
            case Team.Player:
                return Team.Enemy;

            case Team.Enemy:
                return Team.Player;

            default:
                return team;
        }
    }

    /// <summary>
    /// Returns a random team.
    /// </summary>
    public static Team GetRandomTeam() => AllTeams[Random.Range(0, AllTeamsLength)];
}
