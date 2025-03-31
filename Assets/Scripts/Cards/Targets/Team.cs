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
/// Extension methods for the <see cref="Team"/> enum.
/// </summary>
public static class TeamExtensions
{
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
}
