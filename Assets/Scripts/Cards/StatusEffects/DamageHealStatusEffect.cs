using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <see cref="StatusEffect"/>: Heal for X% of all damage you do for the next Y attacks.
/// </summary>
// Script by Ruben
[CreateAssetMenu(fileName = "Damage Heal", menuName = ASSET_MENU_PATH + "Damage Heal")]
public class DamageHealStatusEffect : StatusEffect
{
    public override bool HasPotency => true;

    public override bool? ForcedPotencyIsPercent => true;

    public override string PotencyName => "Percent";

    public override string DurationName => "Attacks";

    public override void OnAttack(Target target, ref float amount)
    {
        if (amount < 0)
        {
            return;
        }

        User.Heal(amount * Potency);

        Duration--;
    }
}
