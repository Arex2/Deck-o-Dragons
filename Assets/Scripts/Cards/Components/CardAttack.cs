using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A <see cref="CardComponent"/> responsible for dealing damage to a <see cref="Target"/>.
/// </summary>
// Script by Ruben
[AddComponentMenu("Card Attack")]
public class CardAttack : CardComponent
{
    /// <summary>
    /// The total damage this Component will deal to a <see cref="Target"/>. <para/>
    /// Takes into account <see cref="Damage"/>, <see cref="DamageMultiplier"/> and <see cref="AttackAmount"/>.
    /// </summary>
    public float TotalDamage => Damage * DamageMultiplier * (float)AttackAmount;

    /// <summary>
    /// The name of the key used to set the <see cref="DamageMultiplier"/> in the <see cref="CardData"/>.
    /// </summary>
    public const string DAMAGE_MULTIPLIER_KEY_NAME = "DAMAGE_MULTIPLIER";

    /// <summary>
    /// All damage is multiplied by this value.
    /// </summary>
    public float DamageMultiplier => GetCardData<float>(DAMAGE_MULTIPLIER_KEY_NAME, 1);

    /// <summary>
    /// How much damage a single attack will deal, does not account for <see cref="DamageMultiplier"/> or <see cref="AttackAmount"/>.
    /// </summary>
    public float Damage => damage[Level];

    /// <summary>
    /// How many attacks this will do.
    /// </summary>
    public int AttackAmount => attackAmount[Level];

    public override TargetFilter TargetFilter => targetFilter;
    [SerializeField] private TargetFilter targetFilter = new(TargetFilter.FilterTeam.Opponent, TargetFilter.FilterMode.Chosen);

    [Space]
    [SerializeField] private UpgradeableFloat damage = new(30, 10);
    [SerializeField] private UpgradeableInt attackAmount = new(1);

    [Space]
    [SerializeField] private CardVFXReference vfx = new("Attack");

    public override void Play(List<Target> targets)
    {

    }

    public override IEnumerator PlayCoroutine(List<Target> targets)
    {
        float damage = Damage * DamageMultiplier;
        int attackAmount = AttackAmount;

        bool bullseye = HasTag(CardManager.BullseyeTag);

        void DoDamage(Target target)
        {
            AttackData data = new(damage, bullseye, target == User);

            User.DoAttack(target, data);

            Target user = User;

            bool addedActionToVFX = false;

            // VFX
            foreach (CardVFX vfx in SpawnVFX(vfx))
            {
                if (vfx == null)
                {
                    continue;
                }

                vfx.SetTarget(target);

                if (vfx.TriggersActions && !addedActionToVFX)
                {
                    vfx.AddAction((cardVFX) => target.Hurt(user, data));
                    addedActionToVFX = true;
                }
            }

            if (!addedActionToVFX)
            {
                target.Hurt(user, data);
            }
        }

        IEnumerator HurtTarget(Target target)
        {
            if (attackAmount <= 1)
            {
                DoDamage(target);
            }
            else
            {
                for (int i = 0; i < attackAmount; i++)
                {
                    DoDamage(target);

                    yield return new WaitForSeconds(0.1f);
                }
            }
        }

        int count = targets.Count;

        if (count == 1)
        {
            yield return HurtTarget(targets[0]);
        }
        else
        {
            foreach (Target target in targets)
            {
                yield return HurtTarget(target);

                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    #region Description Stuff
    [ReplaceDescriptionKeyword]
    private string ReplaceMainKeyword()
    {
        float value = attackAmount.GetValue(Level);
        string result = ReplaceDamageKeyword();

        if (value != 1)
        {
            result += "x" + value.ToString();
        }

        return result;
    }

    [ReplaceDescriptionKeyword("DAMAGE")]
    private string ReplaceDamageKeyword()
    {
        return Mathf.Round(Damage * DamageMultiplier).ToString();
    }

    [ReplaceDescriptionKeyword("AMOUNT")]
    private string ReplaceAttackAmountKeyword()
    {
        return attackAmount.ToString(Level);
    }
    #endregion
}