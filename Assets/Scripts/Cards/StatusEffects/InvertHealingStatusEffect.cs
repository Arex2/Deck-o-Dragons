using UnityEngine;

/// <summary>
/// <see cref="StatusEffect"/>: All healing will be reversed and will instead hurt you for the next X heals.
/// </summary>
// Script by Ruben
[CreateAssetMenu(fileName = "Invert Healing", menuName = ASSET_MENU_PATH + "Invert Healing")]
public class InvertHealingStatusEffect : StatusEffect
{
    public override bool HasPotency => false;

    //public override string DurationName => "Heals";

    public override void OnHeal(HealData healData)
    {
        User.Hurt(new(healData));

        healData.Negate();

        //Duration--;
    }
}
