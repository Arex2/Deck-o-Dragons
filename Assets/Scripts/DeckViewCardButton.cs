using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckViewCardButton : MonoBehaviour
{
    [SerializeField] private DeckLimitPopup popup;
    [SerializeField] private Image deselectOverlay;
    [SerializeField] private float deselectAlpha = 0.3f;
    [Space]
    [SerializeField] private int minNumberOfCardsInDeck = 12;

    private Card card;
    private bool isInActiveDeck;

    public void Initialize(Card card, bool isSelected = true)
    {
        this.card = card;
        if (isSelected)
        {
            SetSelected();
            isInActiveDeck = true;
        }
        else
        {
            SetDeselected();
            isInActiveDeck = false;
        }
    }

    public void ButtonHandle()
    {
        isInActiveDeck = !isInActiveDeck;
        if (isInActiveDeck)
        {
            DeckManager.Instance.RemoveFromBlackList(card);
            SetSelected();
        }
        else if (DeckManager.Instance.Deck.Count - DeckManager.Instance.BlackListCards.Count > minNumberOfCardsInDeck)
        {
            DeckManager.Instance.AddToBlackList(card);
            SetDeselected();
        }
        else
        {
            popup.Popup();
        }
    }
    
    private void SetDeselected()
    {
        deselectOverlay.color = new Color(0,0,0,deselectAlpha);
    }

    private void SetSelected()
    {
        deselectOverlay.color = new Color(0,0,0,0);
    }



}
