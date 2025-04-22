using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;
using UnityEngine.EventSystems;

public class CardObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
{
    public bool AffectOtherCards => Card != null && Card.HasCardComponent<IAffectOtherCards>();
    /*
    public bool affectOtherCards = false;

    //b�r vara i card component??? och b�r kallas p� n�r kort spelas
    //eller possibly ba g�r s� att card targets blir korten?
    private void AffectOtherCards()
    {
        foreach (CardObject cardObj in CardHand.CardsToAffect)
        {
            //do effect
        }
    }
    */

    /// <summary>
    /// Level of the <see cref="card"/>.
    /// </summary>
    public int Tier { get; set; } = 0;

    public int CostOffset { get; set; }

    public float StartYPos { get; set; }

    public CardHand CardHand { get; set; }
    public Canvas Canvas => cardVisuals.Canvas;
    public CardVisuals CardVisuals => cardVisuals;

    [CacheComponent]
    [SerializeField] private CardVisuals cardVisuals;

    [Space]
    [SerializeField] private Target user;
#if UNITY_EDITOR
    // FOR DEBUGGING
    [Space]
    [SerializeField] private Card[] debugCardsToOnlyDraw;
    public bool Debugging => debugCardsToOnlyDraw != null && debugCardsToOnlyDraw.Length > 0;
#endif

    [Space]
    [SerializeField] private float pointerMoveRadius = 0.15f;

    public Action<CardObject> OnCardPressed { get; set; }

    public Card Card { get; private set; }

    private bool _setCard;
    private bool _selected;

    public void Initialize(Card card)
    {
        SetCard(card);
        UpdateCardLook();
    }

    private void Start()
    {
        if (!_setCard)
        {
            BecomeRandomCard();
        }
        //gameObject.GetComponent<SpriteRenderer>().sprite = card.Sprite;
        UpdateCardLook();
    }

    public void UpdateCardLook()
    {
        cardVisuals.UpdateCardLook();
    }

    //Method to update mana cost text when mana affecting cards have been played
    public void UpdateCostLook()
    {
        cardVisuals.UpdateCostLook();
    }

    private void BecomeRandomCard()
    {
#if UNITY_EDITOR
        if (Debugging)
        {
            int debugI = Random.Range(0, debugCardsToOnlyDraw.Length);
            SetCard(debugCardsToOnlyDraw[debugI]);
            return;
        }
#endif

        int i = Random.Range(0, CardManager.DrawableCards.Length);
        SetCard(CardManager.DrawableCards[i]);
    }

    public void SetCard(Card card)
    {
        Card = card;
        cardVisuals.Card = card;
        _setCard = true;
    }

    public int GetCost()
    {
        return Mathf.Max(Card.Cost + CostOffset, 0);
    }

    public void Play()
    {
        Card.Play(user, Tier, OnFinishPlayingCard);
    }

    private void OnFinishPlayingCard()
    {
        Debug.Log("I'm finish");
        //remove mana from target??

        //destroy itself
        StartCoroutine(DeleteItself());
    }

    IEnumerator DeleteItself()
    {
        yield return new WaitForSeconds(1);
        CardHand.cardIsPlaying = false;
        CardHand.cardBeingPlayed = null;
        CardHand.RemoveCard(this);

        Debug.Log("Me rmove");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _selected = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_selected)
        {
            OnCardPressed?.Invoke(this);
        }

        _selected = false;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (!_selected)
            return;


        float dist = Vector2.Distance(eventData.pointerPressRaycast.worldPosition, eventData.pointerCurrentRaycast.worldPosition);

        if (dist < pointerMoveRadius)
            return;

        _selected = false;
    }

    public void CheckmarkAppear()
    {
        cardVisuals.CheckmarkAppear();
    }

    public void CheckmarkDisappear()
    {
        cardVisuals.CheckmarkDisappear();
    }
}
