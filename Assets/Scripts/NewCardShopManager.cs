using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using DG.Tweening;

public class NewCardShopManager : MonoBehaviour
{
    [SerializeField] private int cardsToTake = 3;
    [SerializeField] private int cardsToShow = 6;
    [SerializeField] private int amountOfRerolls = 1;
    
    [Header("Components")]
    [SerializeField] private GameObject contentContainer;
    [SerializeField] private CardShopScroll scroll;
    [SerializeField] private CardViewInctance cardPrefab;
    [SerializeField] private RectTransform zoomCardRectTransform;
    [SerializeField] private CardViewInctance zoomCardViewInctance;
    [SerializeField] private GameObject zoomCardPanel;
    [Header("Texts")]
    [SerializeField] private TMP_Text rerollText;
    [SerializeField] private TMP_Text topText;

    [Header("Buttons")]
    [SerializeField] private Button rerollButton;
    [SerializeField] private Button addButton;

    
    private Vector2 _screenSpaceEnd;
    private float _scaleEnd;
    private Vector2 _screenSpaceStart;
    private float _scaleStart;

    private int _cardsRemaining;
    private int _rerollsRemaining;
    private string _rerollTextFormat;
    private string _topTextFormat;
    private List<Card> _drawableCards = new();
    private List<Card> _cardsBeingShown = new();
    private DeckManager _deckManager;
    private bool isZoomed = false;

    void Start()
    {
        _topTextFormat = topText.text;
        _rerollTextFormat = rerollText.text;

        _rerollsRemaining = amountOfRerolls;

        _deckManager = DeckManager.Instance;

        PopulateCards();
        UpdateText();
        //Do check for no cards to chose

        scroll.Initialize();

        //Setup for the zoom card
        _screenSpaceEnd = zoomCardRectTransform.position;
        _scaleEnd = zoomCardRectTransform.localScale.x;

        _screenSpaceStart = scroll.cardPositions[0].position;
        _scaleStart = scroll.cardPositions[0].localScale.x;
    }

    public void Reroll()
    {
        _rerollsRemaining--;

        ClearCards();
        PopulateCards();

        rerollButton.interactable = _rerollsRemaining > 0;

        UpdateText();
    }

    private void UpdateText()
    {
        topText.text = string.Format(_topTextFormat, _cardsRemaining, cardsToTake);
        rerollText.text = string.Format(_rerollTextFormat, _rerollsRemaining);
    }

    private Card GetRandomCard()
    {
        int length = _drawableCards.Count;

        if (length <= 0)
        {
            return null;
        }

        int index = Random.Range(0, length);

        Card card = _drawableCards[index];

        _drawableCards.RemoveAt(index);

        return card;
    }

    private void AddCard(Card card)
    {
        if(card == null) return;

        CardViewInctance newCard = Instantiate(cardPrefab);
        newCard.setNewCard(card);
        newCard.transform.parent = contentContainer.transform;
        newCard.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

        newCard.gameObject.GetComponent<Button>().onClick.AddListener(delegate {ZoomOnCard();});
        scroll.cardPositions.Add(newCard.GetComponent<RectTransform>());
        _cardsBeingShown.Add(card);
    }

    private void PopulateCards()
    {
        foreach (Card card in CardManager.DrawableCards)
        {
            int cardCount = _deckManager.CardCount(card);

            for (int i = cardCount; i < card.Copies; i++)
            {
                _drawableCards.Add(card);
            }
        }

        int count = _drawableCards.Count;
        while (count > 1)
        {
            count--;
            int index = Random.Range(0, count + 1);
            Card value = _drawableCards[index];
            _drawableCards[index] = _drawableCards[count];
            _drawableCards[count] = value;
        }

        for(int i = 0; i < cardsToShow; i++)
        {
            AddCard(GetRandomCard());
        }
    }

    private void ClearCards()
    {
        foreach (RectTransform cardPosition in scroll.cardPositions)
        {
            Destroy(cardPosition.gameObject);
        }
        scroll.cardPositions.Clear();
        _cardsBeingShown.Clear();
    }

    public void ZoomOnCard()
    {
        if(!isZoomed)
        {
            isZoomed = true;
            zoomCardPanel.SetActive(true);

            zoomCardRectTransform.localScale = Vector3.one * _scaleStart;
            zoomCardRectTransform.position = _screenSpaceStart;

            zoomCardViewInctance.setNewCard(_cardsBeingShown[0]);
            zoomCardRectTransform.DOMove(_screenSpaceEnd, 1.2f);
            zoomCardRectTransform.DOScale(_scaleEnd, 1.2f);
        }
    }

    public void ZoomOut()
    {
        if(isZoomed)
        {
            isZoomed = false;

            zoomCardRectTransform.DOMove(_screenSpaceEnd, 1.2f);
            zoomCardRectTransform.DOScale(_scaleEnd, 1.2f);
            
        }
    }
}
