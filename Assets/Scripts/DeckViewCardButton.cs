using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckViewCardButton : MonoBehaviour
{
    [SerializeField] private CardVisuals cardVisuals;
    [SerializeField] private Image deselectOverlay;
    [Space]
    [SerializeField] private float deselectAlpha = 0.3f;

    private Card card;

    private bool isInActiveDeck;
    

    public void Initialize(Card card)
    {
        this.card = card;

    }
    private void SetUnselected()
    {

    }

    private void SetSelected()
    {

    }



}
