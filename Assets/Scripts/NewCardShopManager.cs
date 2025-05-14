using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewCardShopManager : MonoBehaviour
{
    [SerializeField] private GameObject contentContainer;
    [SerializeField] private CardShopScroll scroll;
    [SerializeField] private CardViewInctance cardPrefab;
    [SerializeField] private TMP_Text topText;
    [SerializeField] private int cardsToTake = 3;
    private int _cardsRemaining;
    [SerializeField] private int cardsToShow = 6;
    [SerializeField] private int amountOfRerolls = 1;
    private int _rerollsRemaining;
    private string _topTextFormat;
    [SerializeField] private TMP_Text rerollText;
    [SerializeField] private Button rerollButton;
    [SerializeField]
    private string _rerollTextFormat;
    private List<Card> _drawableCards = new();
    private List<Card> _cardsBeingShown = new();
    private DeckManager _deckManager;
    void Start()
    {
        _topTextFormat = topText.text;
        _rerollTextFormat = rerollText.text;

        _rerollsRemaining = amountOfRerolls;

        _deckManager = DeckManager.Instance;

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

        UpdateText();
        scroll.Initialize();
        
    }

    public void Reroll()
    {
        _rerollsRemaining--;

        for(int i = 0; i < cardsToShow; i++)
        {
            AddCard(GetRandomCard());
        }

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
        scroll.cardPositions.Add(newCard.GetComponent<RectTransform>());
        _cardsBeingShown.Add(card);
    }
}
