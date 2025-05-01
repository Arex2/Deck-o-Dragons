using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Levels/Change Card Level")]
public class ChangeCardLevel : CardComponent, IAffectOtherCards
{
    [SerializeField] private UpgradeableInt levels = new(1);

    public IEnumerator OnCardsSelected(List<CardObject> cardObjects)
    {
        int level = Level;

        foreach (CardObject cardObj in cardObjects)
        {
            cardObj.Level += levels.GetValue(level);
        }

        return null;
    }

    public CardFilterResult FilterCardObject(CardObject cardObject)
    {
        Card card = cardObject.Card;

        int change = levels[Level];

        if (!card.CanChangeLevel)
        {
            return CardFilterResult.Failure("Can't be " + (change > 0 ? "upgraded" : "downgraded"));
        }

        if (card.MinLevel.Enabled && cardObject.Level <= card.MinLevel.Value)
        {
            return CardFilterResult.Failure("At lowest level, can't downgrade");
        }

        if (card.MaxLevel.Enabled && cardObject.Level >= card.MaxLevel.Value)
        {
            return CardFilterResult.Failure("At MAX level, can't upgrade");
        }

        return CardFilterResult.Success();
    }

    public override void Play(List<Target> targets)
    {

    }

    //[ReplaceDescriptionKeyword]

}
