using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CardShopButton : MonoBehaviour
{
    public CardShop CardShop { get; set; }

    public Card Card { get; private set; }

    [CacheComponent]
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private Image cardImage;
    [SerializeField] private float smallSizeFactor = 0.8f;
    private Transform _transform;
    private Vector3 _startSize;

    private void Awake()
    {
        _transform = transform;
        _startSize = _transform.localScale;

        _transform.localScale = _startSize * smallSizeFactor;
    }

    public void Press()
    {
        CardShop.SelectCardShopButton(this);
    }

    public void SetCard(Card card)
    {
        if (card == null)
        {
            return;
        }

        Card = card;
        UpdateCardLook();
    }

    private void UpdateCardLook()
    {
        cardImage.sprite = Card.Sprite;
    }

    public void Select(bool instant = false)
    {
        Scale(_startSize, instant);
    }

    public void Unselect(bool instant = false)
    {
        Scale(_startSize * smallSizeFactor, instant);
    }

    private void Scale(Vector3 endSize, bool instant = false)
    {
        _transform.DOKill();

        if (instant)
        {
            _transform.localScale = endSize;
        }
        else
        {
            _transform.DOScale(endSize, 0.25f);
        }
    }

    public void Appear()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }

    public void Disappear()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
}
