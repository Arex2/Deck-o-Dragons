using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCardShopManager : MonoBehaviour
{
    [SerializeField] private GameObject contentContainer;
    [SerializeField] private CardShopScroll scroll;
    [SerializeField] private CardViewInctance cardPrefab;

    void Start()
    {
        foreach (Card card in DeckManager.Instance.Deck)
        {
            CardViewInctance newCard = Instantiate(cardPrefab);
            newCard.setNewCard(card);
            newCard.transform.parent = contentContainer.transform;
            newCard.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            scroll.cardPositions.Add(newCard.GetComponent<RectTransform>());
        }

        scroll.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
