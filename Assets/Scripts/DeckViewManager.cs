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

    public void NextPage()
    {
        if (DeckManager.Instance.deck.Count < (currentPage + 1) * 6) return;
        else TurnPage(1);

    }

    public void PreviusPage()
    {
        if ((currentPage - 1) * 6 < 0) return;
        else TurnPage(-1);
    }
    private void TurnPage(int amount) 
    {
        currentPage += amount;

        for (int i = 0; i < cards.Count; i++)
        {
            if (DeckManager.Instance.deck[i + ((currentPage - 1) * 6)] == null)
            {
                cards[i].gameObject.SetActive(false);
            }
            else
            {
                cards[i].gameObject.SetActive(true);
                cards[i].setNewCard(DeckManager.Instance.deck[i + ((currentPage - 1) * 6)]);
            }
                
        }
    }
}
