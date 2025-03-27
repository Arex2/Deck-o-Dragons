using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CardHand : MonoBehaviour
{
    [SerializeField] List<GameObject> cardPositions; //the cards that are shown on screen
    List<Color> cardsInHand = new List<Color>();
    List<object> cardsInDeck;
    int selectedIndex;

    private void PopulateCardsInHand()
    {
        cardsInHand.Add(UnityEngine.Random.ColorHSV());
        cardsInHand.Add(UnityEngine.Random.ColorHSV());
        cardsInHand.Add(UnityEngine.Random.ColorHSV());
        cardsInHand.Add(UnityEngine.Random.ColorHSV());
        cardsInHand.Add(UnityEngine.Random.ColorHSV());
        cardsInHand.Add(UnityEngine.Random.ColorHSV());
        cardsInHand.Add(UnityEngine.Random.ColorHSV());
        cardsInHand.Add(UnityEngine.Random.ColorHSV());
        //cardsInHand.Add("T");
    }

    private void ChooseCard(int index)
    {
        SelectCard(index);
    }
    private void SelectCard(int i)
    {
        //Middle card
        //cardPositions[0].GetComponent<SpriteRenderer>().color = UnityEngine.Random.ColorHSV();
    }
    public void ScrollLeft()
    {
        ++selectedIndex;
        DisplayAllCards();
    }
    public void ScrollRight()
    {
        --selectedIndex;
        DisplayAllCards();
    }
    public void PlayCard()
    {
        //MIDDLE CARD
        cardPositions[2].transform.position = new Vector2(cardPositions[2].transform.position.x, cardPositions[2].transform.position.y+2);
    }

    private void DisplayCard(int index, int cardIndex)
    {
        if (cardIndex < 0 || cardIndex >= cardsInHand.Count)
        {
            cardPositions[index].GetComponent<SpriteRenderer>().enabled = false;
            return;
        }
        cardPositions[index].GetComponent<SpriteRenderer>().enabled = true;
        cardPositions[index].GetComponent<SpriteRenderer>().color = cardsInHand[cardIndex];
    }

    private void DisplayAllCards()
    {
        int j = 0;
        for(int i = selectedIndex-2; i <= selectedIndex+2;  i++)
        {
            //Debug.Log("i " + i + " count:  " + cardPositions.Count + " index:" + selectedIndex);
            //i = card to display
            //j = position to display on 
            DisplayCard(j, i);
            j++;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        PopulateCardsInHand();

        //SELECTS THE INITIAL CARD
        if (cardsInHand != null)
        {
            //Debug.Log("HAND IS NOT NULL");
            if(cardsInHand.Count > 0)
            {
                //selected index = Middle position of cards in hand. If middle is below 1, set to 1
                if ((cardsInHand.Count / 2) < 1)
                    selectedIndex = 0;
                else
                    selectedIndex = Mathf.RoundToInt(cardsInHand.Count / 2);

                //Debug.Log("Selected index: " +selectedIndex + "   " + (cardsInHand.Count / 2));
                //selectedIndex = cardsInHand.Count / 2 < 1 ? 0 : selectedIndex;
                ChooseCard(selectedIndex);
            }
        }

        //Debug.Log("Selected index: " + selectedIndex);
        DisplayAllCards();

        //PlayCard();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
