using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeckViewManager : MonoBehaviour
{
    [SerializeField] private CardViewInctance cardPrefab;
    [SerializeField] private GameObject contentContainer;
    [SerializeField] private TMP_Text activeDeckText;

    private int nbrInActiveDeck = 0;
    void Start()
    {
        List<Card> BlackListTemp = new List<Card>(DeckManager.Instance.BlackListCards);
        foreach (Card card in DeckManager.Instance.Deck)
        {
            CardViewInctance newCard = Instantiate(cardPrefab);
            DeckViewCardButton cardButton = newCard.GetComponent<DeckViewCardButton>();

            newCard.setNewCard(card);
            newCard.transform.SetParent(contentContainer.transform);
            newCard.transform.localScale = new Vector3(1, 1, 1);

            if (cardButton != null)
            {
                if (BlackListTemp.Contains(card))
                {
                    BlackListTemp.Remove(card);
                    cardButton.Initialize(card, false);
                }
                else
                {
                    cardButton.Initialize(card);
                }
            }
        }
    }

    void Update()
    {
        if (nbrInActiveDeck != DeckManager.Instance.Deck.Count - DeckManager.Instance.BlackListCards.Count)
        {
            nbrInActiveDeck = DeckManager.Instance.Deck.Count - DeckManager.Instance.BlackListCards.Count;
            activeDeckText.text = nbrInActiveDeck.ToString();
        }
    }
}
