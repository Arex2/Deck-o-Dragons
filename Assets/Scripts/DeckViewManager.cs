using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckViewManager : MonoBehaviour
{
    [SerializeField] private List<CardViewInctance> cards;
    [SerializeField] private List<Card> testDeck;

    int currentPage = 1;

    private void Start()
    {
        TurnPage(0);
    }
    public void TurnPage(int amount) 
    {
        if (testDeck.Count < (currentPage + 1) * 6) return;

        currentPage += amount;

        for (int i = 0; i < cards.Count; i++)
        {
            if (testDeck[i + ((currentPage - 1) * 6)] == null)
            {
                cards[i].gameObject.SetActive(false);
            }
            else
            {
                cards[i].gameObject.SetActive(true);
                cards[i].setNewCard(testDeck[i + ((currentPage - 1) * 6)]);
            }
                
        }
    }
}
