using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIDeckTracker : MonoBehaviour
{
    [SerializeField] float lerpFactor;
    [SerializeField] TMP_Text drawDisplay;
    [SerializeField] TMP_Text discardDisplay;
    float shownValueDraw = 0f;
    float shownValueDiscard = 0f;
    float targetValueDraw = 0f;
    float targetValueDiscard = 0f;

    void Update()
    {
        targetValueDraw = DeckManager.Instance.DrawPile.Count;
        targetValueDiscard = DeckManager.Instance.DiscardPile.Count;

        shownValueDraw = Mathf.Lerp(shownValueDraw, targetValueDraw, lerpFactor * Time.deltaTime);
        shownValueDiscard = Mathf.Lerp(shownValueDiscard, targetValueDiscard, lerpFactor * Time.deltaTime);

        drawDisplay.text = Mathf.Round(shownValueDraw).ToString();
        discardDisplay.text = Mathf.Round(shownValueDiscard).ToString();
    }

}
