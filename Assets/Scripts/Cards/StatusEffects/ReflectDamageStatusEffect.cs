using UnityEngine;

/// <summary>
/// <see cref="StatusEffect"/>: X% of all damage received will be reflected back to the attacker for the next Y attacks.
/// </summary>
// Script by Ruben
[CreateAssetMenu(fileName = "Reflect Damage", menuName = ASSET_MENU_PATH + "Reflect Damage")]
public class ReflectDamageStatusEffect : StatusEffect
{
    public override bool HasPotency => true;

    public override bool? ForcedPotencyIsPercent => true;

    public override string PotencyName => "Percent";

    //public override string DurationName => "Attacks";

    public override void OnHurt(Target attacker, AttackData attackData)
    {
        if (attackData < 0)
        {
            return;
        }

        if (attacker == null)
        {
            return;
        }

        attacker.NotifyStatusEffects = false;

        attacker.Hurt(attackData * Potency);

        attacker.NotifyStatusEffects = true;

        //Duration--;
    }
}
