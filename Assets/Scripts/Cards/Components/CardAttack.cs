using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A <see cref="CardComponent"/> responsible for dealing damage to a <see cref="Target"/>.
/// </summary>
// Script by Ruben
[AddComponentMenu("Card Attack")]
public class CardAttack : CardComponent, IUseCoroutineMulti
{
    public const string DAMAGE_MULTIPLIER_KEY_NAME = "DAMAGE_MULTIPLIER";

    public float DamageMultiplier => GetCardData<float>(DAMAGE_MULTIPLIER_KEY_NAME, 1);

    public override TargetFilter TargetFilter => targetFilter;

    [SerializeField] private TargetFilter targetFilter = new(TargetFilter.FilterTeam.Opponent, TargetFilter.FilterMode.Chosen);

    [Space]
    [SerializeField] private UpgradeableFloat damage = new(3, 1);
    [SerializeField] private UpgradeableInt attackAmount = new(1);

    public IEnumerator UseCoroutine(List<Target> targets)
    {
        int attackAmount = this.attackAmount[Level];
        float damage = this.damage[Level] * DamageMultiplier;

        void DoDamage(Target target)
        {
            User.DoAttack(target, ref damage);

            target.Hurt(User, damage);
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
        return Mathf.Round(damage.GetValue(Level) * DamageMultiplier).ToString();
    }

    [ReplaceDescriptionKeyword("AMOUNT")]
    private string ReplaceAttackAmountKeyword()
    {
        return attackAmount.ToString(Level);
    }
    #endregion
}