using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardViewInctance : MonoBehaviour
{
    public CardVisuals cardVisuals;
    public void setNewCard(Card card)
    {
        cardVisuals.Card = card;
        cardVisuals.UpdateCardLook();
    }
}
