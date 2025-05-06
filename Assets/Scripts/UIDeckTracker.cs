using DG.Tweening;
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

    [Header("Juice")] // Added by Ruben
    [SerializeField] private Color drawJuiceColor;
    [SerializeField] private Color discardJuiceColor;
    [SerializeField] private Color regularColor;
    [SerializeField] private float juiceScale;
    [SerializeField] private float regularScale;
    [SerializeField] private float juiceDuration;

    float shownValueDraw = 0f;
    float shownValueDiscard = 0f;
    float targetValueDraw = 0f;
    float targetValueDiscard = 0f;

    private string oldDrawDisplayText;
    private string oldDiscardDisplayText;

    private DeckManager deck;

    private void Start()
    {
        deck = DeckManager.Instance;

        OnUpdateDrawPile();
        OnUpdateDiscardPile();

        shownValueDraw = targetValueDraw;
        shownValueDiscard = targetValueDiscard;
    }

    private void OnEnable()
    {
        DeckManager.OnUpdateDrawPile += OnUpdateDrawPile;
        DeckManager.OnUpdateDiscardPile += OnUpdateDiscardPile;
    }

    private void OnDisable()
    {
        DeckManager.OnUpdateDrawPile -= OnUpdateDrawPile;
        DeckManager.OnUpdateDiscardPile -= OnUpdateDiscardPile;
    }

    private void OnUpdateDrawPile()
    {
        if (deck == null) return;

        targetValueDraw = deck.DrawPile.Count;
    }

    private void OnUpdateDiscardPile()
    {
        if (deck == null) return;

        targetValueDiscard = deck.DiscardPile.Count;
    }

    private void Update()
    {
        shownValueDraw = Mathf.Lerp(shownValueDraw, targetValueDraw, lerpFactor * Time.deltaTime);
        shownValueDiscard = Mathf.Lerp(shownValueDiscard, targetValueDiscard, lerpFactor * Time.deltaTime);

        string newDrawDisplayText = Mathf.Round(shownValueDraw).ToString();
        string newDiscardDisplayText = Mathf.Round(shownValueDiscard).ToString();

        if (oldDrawDisplayText != newDrawDisplayText)
        {
            if (!string.IsNullOrEmpty(oldDrawDisplayText))
            {
                JuiceText(drawDisplay, drawJuiceColor);
            }

            oldDrawDisplayText = newDrawDisplayText;
            drawDisplay.text = newDrawDisplayText;
        }
        if (oldDiscardDisplayText != newDiscardDisplayText)
        {
            if (!string.IsNullOrEmpty(oldDiscardDisplayText))
            {
                JuiceText(discardDisplay, discardJuiceColor);
            }

            oldDiscardDisplayText = newDiscardDisplayText;
            discardDisplay.text = newDiscardDisplayText;
        }
    }

    private void JuiceText(TMP_Text text, Color juiceColor)
    {
        // Scale
        text.transform.localScale = Vector3.one * juiceScale;

        text.transform.DOKill();
        text.transform.DOScale(regularScale, juiceDuration).SetEase(Ease.OutSine);

        // Color
        text.color = juiceColor;

        text.DOKill();
        text.DOColor(regularColor, juiceDuration).SetEase(Ease.OutSine);
    }

}
