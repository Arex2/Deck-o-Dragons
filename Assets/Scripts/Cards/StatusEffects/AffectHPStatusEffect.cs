using UnityEngine;

[CreateAssetMenu(fileName = "Affect HP", menuName = ASSET_MENU_PATH + "Affect HP")]
public class AffectHPStatusEffect : StatusEffect
{
    public override bool HasPotency => true;

    public override string PotencyName => IsDebuff ? "Damage" : "Healing";
    public override string DurationName => "Turns";

    public override void OnTurnStart(Target target, StatusEffectData data)
    {
        if (IsDebuff)
        {
            return;
        }

        target.Heal(data.Potency);

        data.Duration--;
    }

    public override void OnTurnEnd(Target target, StatusEffectData data)
    {
        if (!IsDebuff)
        {
            return;
        }

        target.Hurt(data.Potency);

        data.Duration--;
    }
}
