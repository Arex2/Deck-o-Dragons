using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveCards : CardComponent, IAffectOtherCards
{
    public CardHand CardHand
    {
        get
        {
            if (_cardHandCache == null)
            {
                _cardHandCache = FindObjectOfType<CardHand>();
            }

            return _cardHandCache;
        }
    }

    private CardHand _cardHandCache;

    public override void Play(List<Target> targets)
    {

    }

    public IEnumerator OnCardsSelected(List<CardObject> cardObjects)
    {
        foreach (CardObject cardObject in cardObjects)
        {
            cardObject.Dissolve();
            cardObject.Destroy();
            CardHand.DiscardCard(cardObject);
        }

        return null;
    }

    public CardFilterResult FilterCardObject(CardObject cardObject)
    {
        if (cardObject.HasTag(CardManager.BindingTag))
        {
            return CardFilterResult.Failure("Binded cards can't be removed.");
        }

        return CardFilterResult.Success();
    }
}
