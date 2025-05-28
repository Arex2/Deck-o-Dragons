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

    public override void OnTurnEnd()
    {
        if (IsDebuff)
        {
            User.Hurt(new(Potency));
        }
        else
        {
            User.Heal(new(Potency));
        }

        Trigger();
    }
}
