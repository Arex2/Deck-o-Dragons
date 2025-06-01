using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardViewInctance : MonoBehaviour
{
    public CardVisuals cardVisuals;
    public void SetNewCard(Card card)
    {
        cardVisuals.Card = card;
        cardVisuals.UpdateCardLook();
    }

    public void Disolve(Action onFinish)
    {
        cardVisuals.RandomizeDissolve();
        cardVisuals.Dissolve(onFinish);
    }

    public void SetVisible()
    {
        cardVisuals.UIDissolve.DissolveAmount = 0;
    }
}
