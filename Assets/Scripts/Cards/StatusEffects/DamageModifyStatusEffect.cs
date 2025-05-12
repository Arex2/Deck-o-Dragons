using UnityEngine;

/// <summary>
/// <see cref="StatusEffect"/>: Increases/reduces damage of target by X.
/// </summary>
// Script by Ruben
[CreateAssetMenu(fileName = "Damage Modify", menuName = ASSET_MENU_PATH + "Damage Modify")]
public class DamageModifyStatusEffect : StatusEffect
{
    public override bool HasPotency => true;

    public override string PotencyName => PotencyIsPercent ? "Percent" : "Damage";

    //public override string DurationName => "Attacks";

    public override void OnAttack(Target target, AttackData attackData)
    {
        if (attackData.Bullseye && IsDebuff)
        {
            return;
        }

        if (attackData.SelfDamage && !IsDebuff)
        {
            return;
        }

        float potency = Potency * (IsDebuff ? -1f : 1f);

        if (!attackData.Bullseye || !IsDebuff)
        {
            if (PotencyIsPercent)
            {
                attackData.Multiplier += potency;
            }
            else
            {
                attackData += potency;
            }
        }

        //Duration--;
    }
}
