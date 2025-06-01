using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckViewCardButton : MonoBehaviour
{
    [SerializeField] private Image deselectOverlay;
    [SerializeField] private float deselectAlpha = 0.3f;
    [Space]

    public Card card;
    public bool isInActiveDeck;


    public void ButtonHandle()
    {
        isInActiveDeck = !isInActiveDeck;
        if (isInActiveDeck) SetSelected();
        else SetDeselected();
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
