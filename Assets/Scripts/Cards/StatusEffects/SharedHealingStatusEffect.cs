using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <see cref="StatusEffect"/>: For the next X turns, all healing received will also be given to your opponent.
/// </summary>
// Script by Ruben
[CreateAssetMenu(fileName = "Shared Healing", menuName = ASSET_MENU_PATH + "Shared Healing")]
public class SharedHealingStatusEffect : StatusEffect
{
    public override bool HasPotency => false;

    public override string DurationName => "Turns";

    public override void OnHeal(ref float healing)
    {
        Team team = User.Team;

        List<Target> targets = TargetManager.GetTargets(team.GetOpponentTeam());

        foreach (Target target in targets)
        {
            target.NotifyStatusEffects = false;

            target.Heal(healing);

            target.NotifyStatusEffects = true;
        }
    }

    public override void OnTurnEnd()
    {
        Duration--;
    }
}
