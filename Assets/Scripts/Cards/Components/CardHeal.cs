using UnityEngine;

/// <summary>
/// 
/// </summary>
// Script by Ruben
public class CardHeal : CardComponent
{
    [SerializeField] private CardTarget target = CardTarget.Self;

    [Space]
    [SerializeField] private UpgradeableFloat healing = new UpgradeableFloat(2);
}
