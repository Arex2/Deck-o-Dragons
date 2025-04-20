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

    public override string DurationName => "Attacks";

    public override void OnHurt(Target attacker, ref float amount)
    {
        if (amount < 0)
        {
            return;
        }

        attacker.NotifyStatusEffects = false;

        attacker.Hurt(amount * Potency);

        attacker.NotifyStatusEffects = true;

        Duration--;
    }
}
