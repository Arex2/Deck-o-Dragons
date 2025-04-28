using UnityEngine;

/// <summary>
/// <see cref="StatusEffect"/>: Deals/heals X HP every turn for Y turns.
/// </summary>
// Script by Ruben
[CreateAssetMenu(fileName = "Affect HP", menuName = ASSET_MENU_PATH + "Affect HP")]
public class AffectHPStatusEffect : StatusEffect
{
    public override bool HasPotency => true;

    public override string PotencyName => IsDebuff ? "Damage" : "Healing";
    public override string DurationName => "Turns";

    public override void OnTurnStart()
    {
        if (IsDebuff)
        {
            return;
        }

        User.Heal(new(Potency));

        Duration--;
    }

    public override void OnTurnEnd()
    {
        if (!IsDebuff)
        {
            return;
        }

        User.Hurt(new(Potency));

        Duration--;
    }
}
