using UnityEngine;

/// <summary>
/// <see cref="StatusEffect"/>: Increases/reduces defense of target by X.
/// </summary>
[CreateAssetMenu(fileName = "Defense Modify", menuName = ASSET_MENU_PATH + "Defense Modify")]
public class DefenseModifyStatusEffect : StatusEffect
{
    public override bool HasPotency => true;

    public override string PotencyName => PotencyIsPercent ? "Percent" : "Damage";

    public override string DurationName => "Attacks";

    public override void OnHurt(Target attacker, AttackData attackData)
    {
        float potency = Potency * (IsDebuff ? 1f : -1f);

        if (!attackData.Bullseye || IsDebuff)
        {
            if (PotencyIsPercent)
            {
                attackData *= 1 + potency;
            }
            else
            {
                attackData += potency;
            }
        }

        Duration--;
    }
}
