using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <see cref="StatusEffect"/>: Any status effect you receive is also applied to your opponent.
/// </summary>
// Script by Ruben
[CreateAssetMenu(fileName = "Mirror Magic", menuName = ASSET_MENU_PATH + "Mirror Magic")]
public class MirrorMagicStatusEffect : StatusEffect
{
    public override bool HasPotency => false;

    public override string DurationName => "Turns";

    public override void OnTurnEnd()
    {
        Duration--;
    }

    public override void OnOtherStatusEffectApplied(StatusEffect statusEffect, StatusEffectData otherData)
    {
        Team team = User.Team;

        List<Target> targets = TargetManager.GetTargets(team.GetOpponentTeam());

        foreach (Target target in targets)
        {
            target.ApplyStatusEffect(statusEffect, otherData.Clone(), false);
        }
    }
}
