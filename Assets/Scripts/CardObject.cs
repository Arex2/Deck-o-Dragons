using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
{
    public bool affectOtherCards = false;

    //bör vara i card component??? och bör kallas på när kort spelas
    //eller possibly ba gör så att card targets blir korten?
    private void AffectOtherCards()
    {
        foreach (CardObject cardObj in CardHand.CardsToAffect)
        {
            //do effect
        }
    }

    public Canvas Canvas => canvas;

    public Image Checkmark => checkmark;

    public float StartYPos { get; set; }

    public CardHand CardHand { get; set; }

    [SerializeField] private Canvas canvas;
    [Space]

    //Card components
    [SerializeField] private Image background;
    [SerializeField] private Image costBackground;
    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Image checkmark;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text tagsText;
    [SerializeField] private TMP_Text descriptionText;

    [Space]
    [SerializeField] private Card card;
    [SerializeField] private Target user;
#if UNITY_EDITOR
    // FOR DEBUGGING
    [SerializeField] private Card[] debugCardsToOnlyDraw;
    public bool Debugging => debugCardsToOnlyDraw != null && debugCardsToOnlyDraw.Length > 0;
#endif

    [Space]
    [SerializeField] private float pointerMoveRadius = 0.15f;

    public Action<CardObject> OnCardPressed { get; set; }

    private bool _setCard;
    private bool _selected;

    private void Start()
    {
        if (!_setCard)
        {
            BecomeRandomCard();
        }
        //gameObject.GetComponent<SpriteRenderer>().sprite = card.Sprite;
        UpdateCardLook();
    }
    private void UpdateCardLook()
    {
        //ändra background sprite till rätt background depending on tier
        //ändra costBackground till rätt färg depending on cost type
        costBackground.color = Color.blue;
        cardImage.sprite = card.Sprite;
        costText.text = card.Cost.ToString();
        titleText.text = card.DisplayName.ToString();
        UpdateTagText();
        descriptionText.text = card.Description; //.Description returnar inget rn
    }

    private void UpdateTagText()
    {
        tagsText.text = "";
        //Debug.LogWarning("Tags amount: " + card.Tags.Length);
        if (card.Tags == null || card.Tags.Length <= 0)
        {
            tagsText.text = "";
            return;
        }

        string tags = "";
        foreach (CardTag tag in card.Tags)
        {
            if (tag == null)
                continue;
            tags += tag.name;
            tags += " ";
        }
        tagsText.text = tags;
    }

    //Method to update mana cost text when mana affecting cards have been played
    public void UpdateCostLook()
    {
        costText.text = card.Cost.ToString();
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
        this.card = card;
        _setCard = true;
    }

    public int GetCost()
    {
        return card.Cost;
    }

    public void Play()
    {
        card.Play(user, OnFinishPlayingCard);
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
        checkmark.DOKill();
        checkmark.DOFade(1, 0.1f);
    }

    public void CheckmarkDisappear()
    {
        checkmark.DOKill();
        checkmark.DOFade(0, 0.1f);
    }
}
