using UnityEngine;

/// <summary>
/// <see cref="CardComponent"/> that draws a certain amount of card from a specific pool (<see cref="CardTag"/>) or just all cards from an array.
/// </summary>
// Script by Ruben
[AddComponentMenu("Hand/Draw Cards")]
public class DrawCards : CardComponent, IUse
{
    [SerializeField] private CardTag tag;
    [SerializeField] private UpgradeableInt amount = new(1);

    [Space]
    [SerializeField] private Card[] cardsToAlwaysDraw;

    private Card[] _cards;
    private int _cardLength;

    private CardHand _cardHand;

    public override void Initialize()
    {
        _cards = tag == null ? CardManager.DrawableCards : CardManager.GetCardsByTag(tag);
        _cardLength = _cards.Length;
    }

    public void Use()
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

        int amount = this.amount.GetValue(Tier);

        for (int i = 0; i < amount; i++)
        {
            _cardHand.DrawCard(_cards[Random.Range(0, _cardLength)]);
        }

        foreach (Card card in cardsToAlwaysDraw)
        {
            _cardHand.DrawCard(card);
        }
    }

    [ReplaceDescriptionKeyword]
    [ReplaceDescriptionKeyword("CARD_AMOUNT")]
    private string ReplaceAmountKeyword()
    {
        return amount.ToString(Tier);
    }

    [ReplaceDescriptionKeyword("CARD_POOL")]
    private string ReplacePoolKeyword()
    {
        return tag.DisplayName;
    }
}
