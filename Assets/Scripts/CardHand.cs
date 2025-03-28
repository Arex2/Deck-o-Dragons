using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CardHand : MonoBehaviour
{
    int amountToDraw = 6;
    List<GameObject> cardsInHand = new List<GameObject>();
    [SerializeField] GameObject cardPrefab;
    int selectedIndex;
    private void Start()
    {
        SelectInitialCard();
    }

    public void PlayCard() { }

    public void DrawCard() 
    {
        //check if can draw card
        //instantiate new card
        GameObject card = Instantiate(cardPrefab);
        card.GetComponent<SpriteRenderer>().color = UnityEngine.Random.ColorHSV();
        //add to cardsInHand
        cardsInHand.Add(card);
        //update positions
        UpdateCardPositions();
    }

    private void DrawNewHand() 
    { 
        for(int i = 0; i < amountToDraw; i++)
        {
            DrawCard();
        }
    }

    private void SelectInitialCard()
    {
        //selected index = Middle position of cards in hand. If middle is below 1, set to 1
        if ((cardsInHand.Count / 2) < 1)
            selectedIndex = 0;
        else
            selectedIndex = Mathf.RoundToInt(cardsInHand.Count / 2);

    }
    private void UpdateCardPositions() 
    {
        if (cardsInHand.Count == 0)
            return;
        float spacing = 0.8f;
        float firstPos = 0f - spacing * selectedIndex;

        for (int i = 0; i < cardsInHand.Count; i++)
        {
            float posX = firstPos + i * spacing;
            Vector2 newPos = new Vector2(posX, -2f);
            cardsInHand[i].transform.DOMove(newPos, 0.4f);
        }

        /*
        //lägg ut kort till vänster om selected card
        for(int i = 0; i < selectedIndex; i++)
        {
            float posX = firstPos - i * spacing;
            Vector2 newPos = new Vector2(posX, -2f);
            cardsInHand[i].transform.DOMove(newPos, 0.4f);
        }
        for(int i = 0; i < 1; i++)
        {
            //Lägg ut selected card
            float posX = 0;
            Vector2 newPos = new Vector2(posX, -2f);
            cardsInHand[selectedIndex].transform.DOMove(newPos, 0.4f);
        }
        //lägg ut kort till höger om selected card
        for (int i = selectedIndex; i < cardsInHand.Count; i++)
        {
            float posX = firstPos + i * spacing;
            Vector2 newPos = new Vector2(posX, -2f);
            cardsInHand[i].transform.DOMove(newPos, 0.4f);
        }
        */


        /*
        for (int i = 0;i < cardsInHand.Count;i++)
        {
            
            float posX; 
            float centerPos = 0f;
            if(i==0)
            {
                posX = 0;
            }
            else if(i%2 ==0)
            {
                posX = centerPos + i * spacing;
            }
            else
            {
                posX = centerPos - i * spacing;
            }
            
            float posX = firstPos + i * spacing;
            Vector2 newPos = new Vector2(posX,-2f);
            cardsInHand[i].transform.DOMove(newPos, 0.4f);
        }
        */
        UpdateCardLayers();
    }

    private void UpdateCardLayers()
    {
        //vänstra sidan från selected index
        for(int i = 0; i < selectedIndex; i++)
        {
            cardsInHand[i].GetComponent<SpriteRenderer>().sortingOrder = -1 * (selectedIndex - i);
        }
        //selected index
        cardsInHand[selectedIndex].GetComponent<SpriteRenderer>().sortingOrder = 2;
        //högra sidan från selected index
        for(int i = selectedIndex; i < cardsInHand.Count;i++)
        {
            cardsInHand[i].GetComponent<SpriteRenderer>().sortingOrder = -1 * (i -(selectedIndex) +1);
        }
    }



    public void ShiftAllRight()
    {
        if (!CheckIfCardNextTo(-1))
            return;
        --selectedIndex;
        /*
        for(int i = 0; i < cardsInHand.Count-1; i++)
        {
            GameObject temp = cardsInHand[i];
            cardsInHand[i] = cardsInHand[i + 1];
            cardsInHand[i + 1] = temp;
        }
        */
        UpdateCardPositions();
    }
    public void ShiftAllLeft()
    {
        if (!CheckIfCardNextTo(+1))
            return;
        ++selectedIndex;
        /*
        for (int i = 0; i < cardsInHand.Count - 1; i++)
        {
            GameObject temp = cardsInHand[cardsInHand.Count-1];
            cardsInHand[cardsInHand.Count - 1] = cardsInHand[i];
            cardsInHand[i] = temp;
        }
        */
        UpdateCardPositions();
    }

    private bool CheckIfCardNextTo(int direction)
    {
        if (selectedIndex + direction >= cardsInHand.Count || selectedIndex + direction < 0)
        { return false; }
        return true;
    }
    private void ShiftCardsByOne()
    {
        GameObject temp = cardsInHand[0];
        cardsInHand[0] = cardsInHand[cardsInHand.Count-1];
        cardsInHand[cardsInHand.Count - 1] = temp;

        /*
        for(int i = 0; i < cardsInHand.Count; i++) 
        {
            ShiftCard(i);
        }
        */
    }

    private void ShiftCard(int index)
    {
        if (cardsInHand.Count < index + 2)
            return;

        GameObject temp = cardsInHand[index];
        cardsInHand[index] = cardsInHand[index + 2];
        cardsInHand[index +2] = temp;
    }

    public void ChangeSelectedCard()
    {
        //ShiftCardsByOne();
        ShiftAllLeft();
        /*
        GameObject temp = cardsInHand[0];
        cardsInHand[0] = cardsInHand[cardsInHand.Count - 1];
        cardsInHand.RemoveAt(cardsInHand.Count - 1);
        cardsInHand.Add(temp);
        */

        UpdateCardPositions();
    }


    /*
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
        if (!CheckIfCardNextTo(+1))
            return;
        ++selectedIndex;
        DisplayAllCards();
    }
    public void ScrollRight()
    {
        if (!CheckIfCardNextTo(-1))
            return;
        --selectedIndex;
        DisplayAllCards();
    }
    public void PlayCard()
    {
        if (cardsInHand.Count < 0)
        {
            //can't play / NEW HAND
        }
        //MIDDLE CARD
        cardPositions[2].transform.position = new Vector2(cardPositions[2].transform.position.x, cardPositions[2].transform.position.y+2);
        //vänta lite tid, spela animation, och sen
        StartCoroutine(RemovePlayedCard());
    }
    private bool CheckIfCardNextTo(int direction)
    {
        if(selectedIndex + direction >= cardsInHand.Count || selectedIndex + direction < 0)
        {  return false; }
        return true;
    }

    IEnumerator RemovePlayedCard()
    {
        //Wait for 0.4 seconds
        yield return new WaitForSeconds(0.4f);

        cardsInHand.RemoveAt(selectedIndex);
        //return card to origin pos
        cardPositions[2].transform.position = new Vector2(cardPositions[2].transform.position.x, cardPositions[2].transform.position.y - 2);

        if(cardsInHand.Count < 0 )
        {
            //new hand
        }

        Debug.Log("SELECTED     " +selectedIndex);
        //om är vid högra kanten kortet
        if(selectedIndex == cardsInHand.Count)
        {
            //flytta ett till höger (och sortera)
            ScrollRight();
        }
         //Behövs inte för att List<T> automatiskt sorterar bort från lägsta värdet perhaps?
        else if(false)//selectedIndex == 0)
        {
            //flytta ett till vänster (och sortera)
            //ScrollLeft();
            DisplayAllCards();
        }
        
        else
        {
            //sortera bara
            DisplayAllCards();
        }
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
    */
}
