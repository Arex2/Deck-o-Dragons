using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using DG.Tweening;

public class NewCardShopManager : MonoBehaviour
{
    [SerializeField] private bool takeAllCards = true;
    [SerializeField] private int cardsToTake = 3;
    [SerializeField] private int cardsToShow = 6;
    [SerializeField] private int amountOfRerolls = 1;
    [SerializeField] private float timeToZoomIn = 0.6f;
    [SerializeField] private float timeToZoomOut = 0.3f;

    [Header("Components")]
    [SerializeField] private GameObject contentContainer;
    [SerializeField] private CardShopScroll scroll;
    [SerializeField] private CardViewInctance cardPrefab;
    [SerializeField] private RectTransform zoomCardRectTransform;
    [SerializeField] private CardViewInctance zoomCardViewInctance;
    [SerializeField] private GameObject zoomCardPanel;
    [Header("Texts")]
    [SerializeField] private TMP_Text topText;

    [Header("Buttons")]
    [SerializeField] private Button rerollButton;
    [SerializeField] private Button addButton;

    [Header("Sounds")]
    [SerializeField] private AudioClip selectSFX;
    [SerializeField] private AudioClip addSFX;
    [SerializeField] private AudioClip disolveSFX;
    [SerializeField] private AudioClip rerollSFX;

    private SceneSwitcher _sceneSwitcher;
    private Vector2 _screenSpaceEnd;
    private float _scaleEnd;
    private Vector2 _screenSpaceStart;
    private float _scaleStart;

    private int _cardsToTake;
    private int _cardsTaken;
    private int _rerollsRemaining;
    private string _rerollTextFormat;
    private string _topTextFormat;
    private List<Card> _drawableCards = new();
    private List<Card> _cardsBeingShown = new();
    private DeckManager _deckManager;
    private bool isZoomed;

    void Start()
    {
        zoomCardPanel.SetActive(false);
        isZoomed = false;
        
        _topTextFormat = topText.text;

        _rerollsRemaining = amountOfRerolls;
        _cardsTaken = 0;

        _deckManager = DeckManager.Instance;
        _sceneSwitcher = SceneSwitcher.Instance;

        PopulateCards();

        _cardsToTake = _drawableCards.Count < cardsToTake ? _drawableCards.Count : cardsToTake;

        UpdateText();
        //Do check for no cards to chose

        scroll.Initialize();

        if (!takeAllCards) Exit();


        //Setup for the zoom card
        StartCoroutine(SetCardPosition());
    }

    public void Reroll()
    {
        _rerollsRemaining--;

        ClearCards();
        PopulateCards();
        AudioManager.Instance.PlaySFX(rerollSFX);

        rerollButton.interactable = _rerollsRemaining > 0;

    }

    private void UpdateText()
    {
        topText.text = string.Format(_topTextFormat, _cardsTaken, _cardsToTake);
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
        if (card == null) return;

        CardViewInctance newCard = Instantiate(cardPrefab);
        newCard.SetNewCard(card);
        newCard.transform.parent = contentContainer.transform;
        newCard.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

        newCard.gameObject.GetComponent<Button>().onClick.AddListener(delegate { ZoomOnCard(); });
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

        for (int i = 0; i < cardsToShow; i++)
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

    IEnumerator SetCardPosition()
    {
        yield return new WaitForSeconds(0.6f);
        _screenSpaceEnd = zoomCardRectTransform.position;
        _scaleEnd = zoomCardRectTransform.localScale.x;

        _screenSpaceStart = scroll.cardPositions[0].position;
        _scaleStart = scroll.cardPositions[0].localScale.x;
    }

    public void ZoomOnCard()
    {
        if (!isZoomed)
        {
            isZoomed = true;
            zoomCardPanel.SetActive(true);

            zoomCardRectTransform.localScale = Vector3.one * _scaleStart;
            zoomCardRectTransform.position = _screenSpaceStart;

            zoomCardViewInctance.SetNewCard(_cardsBeingShown[scroll.indexOfShortestDistance]);
            zoomCardRectTransform.DOMove(_screenSpaceEnd, timeToZoomIn);
            zoomCardRectTransform.DOScale(_scaleEnd, timeToZoomIn);
            AudioManager.Instance.PlaySFX(selectSFX);
        }
    }

    public void ZoomOut()
    {
        if (isZoomed)
        {
            isZoomed = false;

            zoomCardRectTransform.DOMove(_screenSpaceStart, timeToZoomOut);
            zoomCardRectTransform.DOScale(_scaleStart, timeToZoomOut);
            Invoke("CloseZoomWindow", timeToZoomOut);
            AudioManager.Instance.PlaySFX(selectSFX);
        }
    }

    private void CloseZoomWindow()
    {
        zoomCardViewInctance.SetVisible();
        zoomCardPanel.SetActive(false);
    }

    public void AddCardToDeck()
    {
        if (!isZoomed) return;

        _cardsTaken++;
        Card cardToAdd = _cardsBeingShown[scroll.indexOfShortestDistance];
        DeckManager.Instance.AddCardToDeck(cardToAdd, 1);
        _cardsBeingShown.Remove(cardToAdd);
        scroll.RemoveCurrentCard();

        if (_cardsTaken >= _cardsToTake)
        {
            addButton.interactable = false;
            Invoke("Exit", 1.4f);
        }

        UpdateText();
        AudioManager.Instance.PlaySFX(addSFX);
        AudioManager.Instance.PlaySFX(disolveSFX);
        zoomCardViewInctance.Disolve(CloseZoomWindow);

        isZoomed = false;
    }

    private void Exit()
    {
        SceneSwitcher.SwitchScene(SceneSwitcher.GetScene(SceneSwitcher.Scene.Egg));
    }
}
