using UnityEngine;

/// <summary>
/// <see cref="StatusEffect"/>: Your next attack will deal +X extra damage. This increases by +Y every time you get hit. Disappears after Z turns or when you attack.
/// </summary>
// Script by Ruben
[CreateAssetMenu(fileName = "RAGE", menuName = ASSET_MENU_PATH + "RAGE")]
public class RageStatusEffect : StatusEffect
{
    public override bool HasPotency => true;

    public override bool? ForcedPotencyIsPercent => false;

    public override string PotencyName => "DMG per hit";
    //public override string DurationName => "Turns";

    public override void OnHurt(Target attacker, AttackData attackData)
    {
        if (attackData <= 0)
        {
            return;
        }

        if (!SetupUserData)
        {
            UserData = (int)1;
            return;
        }

        int count = (int)UserData;

        UserData = count + 1;
    }

    public override void OnAttack(Target target, AttackData attackData)
    {
        if (!SetupUserData)
        {
            return;
        }

        if ((int)UserData <= (int)0)
        {
            return;
        }

        attackData += (float)((int)UserData) * Potency;
        UserData = (int)0;
    }

    /*
    public override void OnTurnEnd()
    {
        Duration--;
    }
    */

    public override string DefaultUserDataString()
    {
        return "0";
    }
}
