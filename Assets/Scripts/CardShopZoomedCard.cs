using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CardShopZoomedCard : MonoBehaviour
{
    [SerializeField] private RectTransform cardRectTransform;
    [SerializeField] private CardViewInctance cardViewInctance;
    private Vector2 screenSpaceEnd;
    private float scaleEnd;
    public Vector2 screenSpaceStart;
    public float scaleStart;

    void Start()
    {
        screenSpaceEnd = GetComponent<RectTransform>().position;
        scaleEnd = GetComponent<RectTransform>().localScale.x;
    }

    public void Setup(RectTransform startRectTransform)
    {
        screenSpaceStart = startRectTransform.position;
        scaleStart = startRectTransform.localScale.x;
    }

    public void ZoomOnCard(Card card)
    {
        cardRectTransform.localScale = Vector3.one * scaleStart;
        cardRectTransform.position = screenSpaceStart;

        cardViewInctance.setNewCard(card);
        cardRectTransform.DOMove(screenSpaceEnd, 1.2f);
        cardRectTransform.DOScale(scaleEnd, 1.2f);
    }

    public void ZoomOut()
    {

    }
}
