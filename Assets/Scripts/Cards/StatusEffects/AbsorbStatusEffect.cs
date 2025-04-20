using UnityEngine;

/// <summary>
/// <see cref="StatusEffect"/>: Shield that absorbs X damage. Disappears after Y turns.
/// </summary>
// Script by Ruben
[CreateAssetMenu(fileName = "Absorb", menuName = ASSET_MENU_PATH + "Absorb")]
public class AbsorbStatusEffect : StatusEffect
{
    public override bool HasPotency => true;

    public override string PotencyName => "HP";

    public override string DurationName => "Turns";

    public override void OnHurt(Target attacker, ref float amount)
    {
        if (amount > Potency)
        {
            amount -= Potency;

            Potency = 0;
        }
        else if (amount <= Potency)
        {
            Potency -= amount;
            amount = 0;
        }

        if (Potency <= 0)
        {
            Remove();
        }
    }

    public override void OnTurnEnd()
    {
        Duration--;
    }
}
