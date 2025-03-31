using System.Collections;
using UnityEngine;

/// <summary>
/// 
/// </summary>
// Script by Ruben
public class CardHeal : CardComponent
{
    [SerializeField] private Optional<TargetFilter> overrideTarget = new(true, new(TargetFilter.FilterTeam.Own, TargetFilter.FilterMode.Leader));

    [Space]
    [SerializeField] private UpgradeableFloat healing = new UpgradeableFloat(2);

    public override IEnumerator Play()
    {
        yield return null;
    }
}
