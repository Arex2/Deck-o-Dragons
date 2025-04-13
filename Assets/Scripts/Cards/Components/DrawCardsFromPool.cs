using UnityEngine;

/// <summary>
/// <see cref="CardComponent"/> that will add a new card to the players hand.
/// </summary>
// Script by Ruben
public class DrawCardsFromPool : CardComponent, IUse
{
    [SerializeField] private CardTag tag;
    [SerializeField] private UpgradeableInt amount = new(1);

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
    [ReplaceDescriptionKeyword("CARD_POOL")]
    private string ReplacePoolKeyword()
    {
        return tag.DisplayName;
    }

    [ReplaceDescriptionKeyword]
    [ReplaceDescriptionKeyword("CARD_AMOUNT")]
    private string ReplaceAmountKeyword()
    {
        return amount.ToString(Tier);
    }
}
