using UnityEngine;

/// <summary>
/// <see cref="StatusEffect"/>: Increases/reduces all healing by X for the next Y heals.
/// </summary>
// Script by Ruben
[CreateAssetMenu(fileName = "Healing Modify", menuName = ASSET_MENU_PATH + "Healing Modify")]
public class HealingModifyStatusEffect : StatusEffect
{
    public override bool HasPotency => true;

    public override string PotencyName => PotencyIsPercent ? "Percent" : "Healing";

    public override string DurationName => "Heals";

    public override void OnHeal(HealData healData)
    {
        float potency = Potency * (IsDebuff ? -1f : 1f);

        if (PotencyIsPercent)
        {
            healData *= 1 + potency;
        }
        else
        {
            healData += potency;
        }

        Duration--;
    }
}
