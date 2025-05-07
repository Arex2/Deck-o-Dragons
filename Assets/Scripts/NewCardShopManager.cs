using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCardShopManager : MonoBehaviour
{
    [SerializeField] private GameObject contentContainer;
    [SerializeField] private GameObject zoomContainer;
    [SerializeField] private CardViewInctance cardPrefab;

    void Start()
    {
        foreach (Card card in DeckManager.Instance.Deck)
        {
            CardViewInctance newCard = Instantiate(cardPrefab);
            newCard.setNewCard(card);
            newCard.transform.parent = contentContainer.transform;
            newCard.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
