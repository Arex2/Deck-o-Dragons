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
    [SerializeField] private Card[] cardsToDraw;

    private Card[] _cards;

    public override void Initialize()
    {
        _cards = tag == null ? CardManager.AllCards : CardManager.GetCardsByTag(tag);
    }

    public void Use()
    {
        CardHand hand = FindObjectOfType<CardHand>();

        int amount = this.amount.GetValue(Tier);

        for (int i = 0; i < amount; i++)
        {
            hand.DrawCard();
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
