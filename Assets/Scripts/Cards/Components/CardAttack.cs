using System.Collections;
using UnityEngine;

/// <summary>
/// 
/// </summary>
// Script by Ruben
public class CardAttack : CardComponent
{
    [SerializeField] private Optional<TargetFilter> overrideTarget = new(true, new(TargetFilter.FilterTeam.Opponent, TargetFilter.FilterMode.Chosen));

    [Space]
    [SerializeField] private UpgradeableFloat damage = new UpgradeableFloat(3, 1);
    [SerializeField] private UpgradeableFloat attackAmount = new UpgradeableFloat(1);
    [SerializeField] private Optional<Element> overrideElement;

    public override void Initialize()
    {

    }

    public override IEnumerator Play()
    {
        yield return null;
    }
}