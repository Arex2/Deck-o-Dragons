using UnityEngine;

public class DrawCard : CardComponent, IUse
{
    //[SerializeField] private Card[] cardsToDraw;
    [SerializeField] private UpgradeableFloat tempAmount;

    public void Use()
    {
        // TODO: ADD OPTION TO DRAW CARDS FROM AN ARRAY FIELD
        CardHand hand = FindObjectOfType<CardHand>();

        int amount = Mathf.RoundToInt(tempAmount.GetValue(Tier));

        for (int i = 0; i < amount; i++)
        {
            hand.DrawCard();
        }
    }

    [ReplaceDescriptionKeyword]
    [ReplaceDescriptionKeyword("CARD_AMOUNT")]
    private string ReplaceDescriptionKeyword()
    {
        return tempAmount.ToString(Tier);
    }
}
