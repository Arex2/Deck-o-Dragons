using System.Collections;
using UnityEngine;

/// <summary>
/// 
/// </summary>
// Script by Ruben
public class CardHeal : CardComponent, IUseSingle
{
    public override TargetFilter TargetFilter => targetFilter;
    [SerializeField] private TargetFilter targetFilter = new(TargetFilter.FilterTeam.Own, TargetFilter.FilterMode.Leader);

    [Space]
    [SerializeField] private UpgradeableFloat healing = new UpgradeableFloat(2);

    public void Use(Target target)
    {
        target.Heal(healing.GetValue(Tier));
    }

    [ReplaceDescriptionKeyword]
    [ReplaceDescriptionKeyword("HEALING")]
    private string ReplaceDescriptionKeyword()
    {
        return healing.ToString(Tier);
    }
}
