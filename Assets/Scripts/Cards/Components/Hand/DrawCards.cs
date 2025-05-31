using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <see cref="CardComponent"/> that draws a certain amount of card from a specific pool (<see cref="CardTag"/>) or just all cards from an array.
/// </summary>
// Script by Ruben
[AddComponentMenu("Hand/Draw Cards")]
public class DrawCards : CardComponent
{
    [SerializeField] private CardTag tag;
    [SerializeField] private UpgradeableInt amount = new(1);

    [Space]
    [SerializeField] private CardCopyData[] cardsToAlwaysDraw;

    private CardHand _cardHand;
    private DeckManager _deckManager;

    public override void Play(List<Target> targets)
    {
        if (_cardHand == null)
        {
            _cardHand = CardHand.Instance;

            if (_cardHand == null)
            {
                Debug.LogWarning($"There is no {nameof(CardHand)} in the scene!");
                return;
            }
        }

        if (_deckManager == null)
        {
            _deckManager = DeckManager.Instance;
        }

        int amount = this.amount.GetValue(Level);

        if (amount > 0)
        {
            for (int i = 0; i < amount; i++)
            {
                if (!_deckManager.CanDrawNext())
                {
                    break;
                }

                Card card = tag != null ? _deckManager.DrawNextWithTag(tag) : _deckManager.DrawNext();

                if (card == null)
                {
                    continue;
                }

                _cardHand.DrawCard(card);
            }
        }

        foreach (CardCopyData cardCopyData in cardsToAlwaysDraw)
        {
            foreach (Card card in cardCopyData)
            {
                _cardHand.DrawCard(card);
            }
        }
    }

    [ReplaceDescriptionKeyword]
    [ReplaceDescriptionKeyword("CARD_AMOUNT")]
    private string ReplaceAmountKeyword()
    {
        return amount.ToString(Level);
    }

    [ReplaceDescriptionKeyword("CARD_POOL")]
    private string ReplacePoolKeyword()
    {
        return tag.DisplayName;
    }
}
