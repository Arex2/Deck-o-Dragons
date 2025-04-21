using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Upgrading/Upgrade Card")]
public class UpgradeCard : CardComponent, IAffectOtherCards
{
    [SerializeField] private UpgradeableInt count = new(1);
    [SerializeField] private UpgradeableInt levels = new(1);

    public int Count => count.GetValue(Tier);

    public IEnumerator OnCardsSelected(CardObject[] cardObjects)
    {
        int tier = Tier;

        foreach (CardObject cardObj in cardObjects)
        {
            cardObj.Tier += levels.GetValue(tier);
        }

        return null;
    }
}
