using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A <see cref="CardComponent"/> responsible for giving <see cref="Target"/> block.
/// </summary>
// Script by Johannes
public class CardBlock : CardComponent, IUseSingle
{
    public override TargetFilter TargetFilter => targetFilter;
    [SerializeField] private TargetFilter targetFilter = new(TargetFilter.FilterTeam.Own, TargetFilter.FilterMode.Leader);

    [Space]
    [SerializeField] private UpgradeableFloat block = new UpgradeableFloat(2);
    public void Use(Target target)
    {
        target.AddBlock(block.GetValue(Tier));
    }

    [ReplaceDescriptionKeyword]
    [ReplaceDescriptionKeyword("BLOCK")]
    private string ReplaceDescriptionKeyword()
    {
        return block.ToString(Tier);
    }
}
