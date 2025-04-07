using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A <see cref="CardComponent"/> responsible for dealing damage to a <see cref="Target"/>.
/// </summary>
// Script by Ruben
public class CardAttack : CardComponent, IUseCoroutineMulti
{
    public override TargetFilter TargetFilter => targetFilter;
    [SerializeField] private TargetFilter targetFilter = new(TargetFilter.FilterTeam.Opponent, TargetFilter.FilterMode.Chosen);

    [Space]
    [SerializeField] private UpgradeableFloat damage = new UpgradeableFloat(3, 1);
    [SerializeField] private UpgradeableFloat attackAmount = new UpgradeableFloat(1);

    public IEnumerator UseCoroutine(List<Target> targets)
    {
        int attackAmount = Mathf.RoundToInt(this.attackAmount.GetValue(Tier));
        float damage = this.damage.GetValue(Tier);

        IEnumerator HurtTarget(Target target)
        {
            if (attackAmount <= 1)
            {
                target.Hurt(damage);
            }
            else
            {
                for (int i = 0; i < attackAmount; i++)
                {
                    target.Hurt(damage);

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
        float value = attackAmount.GetValue(Tier);

        if (value != 1)
        {
            return damage.ToString(Tier) + "x" + value.ToString();
        }

        return damage.ToString(Tier);
    }

    [ReplaceDescriptionKeyword("DAMAGE")]
    private string ReplaceDamageKeyword()
    {
        return damage.ToString(Tier);
    }

    [ReplaceDescriptionKeyword("AMOUNT")]
    private string ReplaceAttackAmountKeyword()
    {
        return attackAmount.ToString(Tier);
    }
    #endregion
}