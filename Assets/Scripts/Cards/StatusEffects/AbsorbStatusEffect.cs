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

    public override void OnHurt(Target attacker, AttackData attackData)
    {
        if (attackData > Potency)
        {
            attackData -= Potency;

            Potency = 0;
        }
        else if (attackData <= Potency)
        {
            Potency -= attackData;
            attackData.Negate("Absorbed");
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
