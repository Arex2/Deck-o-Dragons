using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCardCost : CardComponent, IAffectOtherCards
{
    [SerializeField] private Upgradeable<UpgradeableInt.Method> method = new(default, 0, 0);
    [SerializeField] private UpgradeableInt costModifier = new(0);

    public override void Play(List<Target> targets)
    {
        
    }

    public IEnumerator OnCardsSelected(List<CardObject> cardObjects)
    {
        int costModifier = this.costModifier.GetValue(Level);
        UpgradeableInt.Method method = this.method.GetValue(Level);

        foreach (CardObject cardObj in cardObjects)
        {
            switch (method)
            {
                case UpgradeableNumber<int>.Method.Add:
                    cardObj.Cost += costModifier;
                    break;

                case UpgradeableNumber<int>.Method.Subtract:
                    cardObj.Cost -= costModifier;
                    break;

                case UpgradeableNumber<int>.Method.Multiply:
                    cardObj.Cost *= costModifier;
                    break;

                case UpgradeableNumber<int>.Method.Divide:
                    if (costModifier == 0)
                    {
                        Debug.LogWarning("Dividing by zero! Setting cost to 0.");
                        cardObj.Cost = 0;
                    }
                    else
                    {
                        cardObj.Cost /= costModifier;
                    }
                    break;

                case UpgradeableNumber<int>.Method.Override:
                    cardObj.Cost = costModifier;
                    break;
            }
        }

        return null;
    }

    public CardFilterResult FilterCardObject(CardObject cardObject)
    {
        return CardFilterResult.Success();
    }
}
