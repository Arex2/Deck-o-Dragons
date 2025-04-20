using UnityEngine;

/// <summary>
/// <see cref="StatusEffect"/>: Gives a X% chance to negate all damage for the next Y attacks.
/// </summary>
// Script by Ruben
[CreateAssetMenu(fileName = "Dodge", menuName = ASSET_MENU_PATH + "Dodge")]
public class DodgeStatusEffect : StatusEffect
{
    public override bool HasPotency => true;

    public override bool? ForcedPotencyIsPercent => true;

    public override string PotencyName => "Percent";

    public override string DurationName => "Attacks";

    public override void OnHurt(Target attacker, ref float amount)
    {
        if (Random.value <= Potency)
        {
            amount = 0;
        }

        Duration--;
    }
}
