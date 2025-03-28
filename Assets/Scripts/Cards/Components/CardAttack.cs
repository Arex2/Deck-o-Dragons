using UnityEngine;

/// <summary>
/// 
/// </summary>
// Script by Ruben
public class CardAttack : CardComponent
{
    [SerializeField] private CardTarget target = CardTarget.Enemy;

    [Space]
    [SerializeField] private UpgradeableFloat damage = new UpgradeableFloat(3, 1);
    [SerializeField] private Optional<Element> overrideElement;

    public override void Initialize()
    {
        for (int i = -10; i <= 10; i++)
        {
            Debug.Log("Tier " + i + " | " + damage.GetValue(i));
        }
    }
}