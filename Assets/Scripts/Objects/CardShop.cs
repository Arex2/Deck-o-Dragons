using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// The object that handles adding new cards to your deck. <para/>
/// NOT REALLY A SHOP I KNOW BUT IDK WHAT ELSE TO CALL IT.
/// </summary>
public class CardShop : MonoBehaviour
{
    [SerializeField]
    private CardShopButton cardShopButtonPrefab;
    [SerializeField]
    private Transform[] rows;

    [SerializeField] private int countPerRow = 3;

    [SerializeField] private int cardsToTake = 5;
    private int _cardsRemaining;
    [SerializeField] private int amountOfRerolls = 5;
    private int _rerollsRemaining;

    [Space]
    [SerializeField] private Button rerollButton;
    [SerializeField] private Button addButton;

    [Space]
    [SerializeField] private TMP_Text topText;
    private string _topTextFormat;
    [SerializeField] private TMP_Text rerollText;
    private string _rerollTextFormat;
    [SerializeField] private TMP_Text cardNameText;
    [SerializeField] private TMP_Text cardTagsText;
    [SerializeField] private TMP_Text cardCostText;
    private string _cardCostFormat;
    [SerializeField] private TMP_Text cardDescriptionText;

    [Space]
    [SerializeField] private GameObject regularUIParent;
    [SerializeField] private GameObject selectedCardUIParent;

    [Space]
    [SerializeField] private int exitScene;
    [SerializeField] private SceneSwitcher sceneSwitcher;

    public CardShopButton Selected { get; private set; }

    private CardShopButton[] _cardShopButtons;
    private int _cardShopButtonsCount;

    private List<Card> _drawableCards = new();
    private DeckManager _deckManager;

    private void Start()
    {
        _topTextFormat = topText.text;
        _rerollTextFormat = rerollText.text;
        _cardCostFormat = cardCostText.text;

        _deckManager = DeckManager.Instance;

        _cardsRemaining = cardsToTake;

        foreach (Card card in CardManager.DrawableCards)
        {
            int cardCount = _deckManager.CardCount(card);

            for (int i = cardCount; i < card.Copies; i++)
            {
                _drawableCards.Add(card);
            }
        }

        // Shuffle
        int count = _drawableCards.Count;
        while (count > 1)
        {
            count--;
            int index = Random.Range(0, count + 1);
            Card value = _drawableCards[index];
            _drawableCards[index] = _drawableCards[count];
            _drawableCards[count] = value;
        }

        int rowCount = rows.Length;

        _cardShopButtonsCount = rowCount * countPerRow;
        _cardShopButtons = new CardShopButton[_cardShopButtonsCount];
        int cardShopButtonIndex = 0;

        foreach (Transform row in rows)
        {
            for (int i = 0; i < countPerRow; i++, cardShopButtonIndex++)
            {
                CardShopButton newCardShopButton = Instantiate(cardShopButtonPrefab, row);

                newCardShopButton.gameObject.SetActive(true);
                newCardShopButton.CardShop = this;

                _cardShopButtons[cardShopButtonIndex] = newCardShopButton;
            }
        }

        cardShopButtonPrefab.gameObject.SetActive(false);

        UpdateUI();

        _rerollsRemaining = amountOfRerolls + 1;

        Reroll();
    }

    private void UpdateText()
    {
        topText.text = string.Format(_topTextFormat, _cardsRemaining, cardsToTake);
        rerollText.text = string.Format(_rerollTextFormat, _rerollsRemaining);
    }

    public void Reroll()
    {
        if (_rerollsRemaining <= 0)
        {
            rerollButton.interactable = false;
            return;
        }

        _rerollsRemaining--;

        foreach (CardShopButton cardShopButton in _cardShopButtons)
        {
            Card card = GetRandomCard();

            if (card == null)
            {
                cardShopButton.Disappear();
            }
            else
            {
                cardShopButton.Appear();
                cardShopButton.SetCard(card);
            }
        }

        rerollButton.interactable = _rerollsRemaining > 0;

        UpdateText();
    }

    private Card GetRandomCard()
    {
        int length = _drawableCards.Count;

        if (length <= 0)
        {
            return null;
        }

        int index = Random.Range(0, length);
        /*
        Card card;
        int tries = 0;

        do
        {
            card = _drawableCards[index];

            index++;
            tries++;
        }
        while (card.Rarity != rarity && tries < length);

        if (card.Rarity != rarity)
        {
            return null;
        }
        */

        Card card = _drawableCards[index];

        _drawableCards.RemoveAt(index);

        return card;
    }

    public void ConfirmCardShopButton()
    {
        if (_cardsRemaining <= 0)
        {
            addButton.interactable = false;
            return;
        }

        if (Selected == null)
        {
            return;
        }

        Selected.Unselect();

        Card card = Selected.Card;

        Selected.Disappear();
        Selected = null;

        _cardsRemaining--;

        addButton.interactable = _cardsRemaining > 0;

        _deckManager.AddCardToDeck(card, card.Copies);

        UpdateText();

        UpdateUI();
    }

    public void CancelCardShopButton()
    {
        if (Selected != null)
        {
            Selected.Unselect();
        }

        Selected = null;

        UpdateUI();
    }

    public void SelectCardShopButton(CardShopButton cardShopButton)
    {
        if (Selected == cardShopButton)
        {
            return;
        }

        if (Selected != null)
        {
            Selected.Unselect();
        }

        Selected = cardShopButton;
        Selected.Select();

        UpdateUI();
    }

    private void UpdateUI()
    {
        bool cardUI = Selected != null;

        regularUIParent.SetActive(!cardUI);
        selectedCardUIParent.SetActive(cardUI);

        if (cardUI)
        {
            Card card = Selected.Card;

            cardNameText.text = card.DisplayName;

            cardTagsText.text = CardVisuals.GetTagsString(card);

            cardDescriptionText.text = card.GetDescription(0);

            cardCostText.text = string.Format(_cardCostFormat, card.Cost);
        }
    }

    public void Exit()
    {
        sceneSwitcher.LoadEgg(6);
    }
}
