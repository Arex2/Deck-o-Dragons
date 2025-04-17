using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Damage Modify", menuName = ASSET_MENU_PATH + "Damage Modify")]
public class DamageModifyStatusEffect : StatusEffect
{
    public override bool HasPotency => true;

    public override string PotencyName => PotencyIsPercent ? "Percent" : "Damage";

    public override string DurationName => "Attacks";

    public override void OnAttack(Target user, StatusEffectData data, Target target, ref float amount)
    {
        float potency = data.Potency * (IsDebuff ? -1f : 1f);

        if (PotencyIsPercent)
        {
            amount *= 1 + potency;
        }
        else
        {
            amount += potency;
        }

        data.Duration--;
    }
}
