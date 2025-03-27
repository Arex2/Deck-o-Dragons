using UnityEngine;

/// <summary>
/// 
/// </summary>
// Script by Ruben
public class CardAttack : CardComponent
{
    [SerializeField] private CardTarget target = CardTarget.Enemy;

    [Space]
    [SerializeField] private int damage;
    [SerializeField] private Optional<Element> overrideElement;
}