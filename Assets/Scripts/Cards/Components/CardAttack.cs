using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
// Script by Ruben
public class CardAttack : CardComponent, IUseCoroutineSingle
{
    public override TargetFilter TargetFilter => targetFilter;
    [SerializeField] private TargetFilter targetFilter = new(TargetFilter.FilterTeam.Opponent, TargetFilter.FilterMode.Chosen);

    [Space]
    [SerializeField] private UpgradeableFloat damage = new UpgradeableFloat(3, 1);
    [SerializeField] private UpgradeableFloat attackAmount = new UpgradeableFloat(1);
    [SerializeField] private Optional<Element> overrideElement;

    public IEnumerator UseCoroutine(Target target)
    {
        int count = Mathf.RoundToInt(attackAmount.GetValue(Tier));

        for (int i = 0; i < count; i++)
        {
            target.Hurt(damage.GetValue(Tier));
        }

        yield return new WaitForSeconds(0.1f);
    }

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
}