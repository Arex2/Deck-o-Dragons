using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckViewCardButton : MonoBehaviour
{
    [SerializeField] private CardVisuals cardVisuals;
    [SerializeField] private Image deselectOverlay;

    private Card _card;

    private bool isInActiveDeck;

}
