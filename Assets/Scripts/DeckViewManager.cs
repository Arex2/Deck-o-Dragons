using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckViewManager : MonoBehaviour
{
    [SerializeField] private CardViewInctance cardPrefab;
    [SerializeField] private GameObject contentContainer;

    void Start()
    {
        foreach (Card card in DeckManager.Instance.Deck)
        {
            CardViewInctance newCard = Instantiate(cardPrefab);
            DeckViewCardButton cardButton = newCard.GetComponent<DeckViewCardButton>();

            newCard.setNewCard(card);
            newCard.transform.parent = contentContainer.transform;
            newCard.transform.localScale = new Vector3(1, 1, 1);

            if(cardButton != null)
            {
                cardButton.Initialize(card);
            }
        }

    }
}
