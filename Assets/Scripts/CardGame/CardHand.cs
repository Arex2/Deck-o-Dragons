using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CardHand : MonoBehaviour
{
    [SerializeField]
    private GameBehaviour gameBehaviour;

    int amountToDraw = 6;
    int selectedIndex;

    /*
    //för att få current pos av cards
    public int SelectedIndex
    { get { return selectedIndex; } }

    public Vector2 CurrentPos
    {
        get { if (cardsInHand.Count > 0) 
                 return new Vector2(cardsInHand[0].transform.position.x, 0);
            else return new Vector2(0,0);
        }
    }
    */

    private int startPoint;
    public void SetStart()
    {
        startPoint = selectedIndex;
    }


    [SerializeField] GameObject cardPrefab;
    [SerializeField] Card testCard;
    Vector3 cardPlayPosition = new Vector3(0, 0.5f, 0);
    Vector3 cardPlayErrorPosition = new Vector3(0, 0.5f, 0);
    List<GameObject> cardsInHand = new List<GameObject>();
    List<GameObject> cardsSelected = new List<GameObject>();

    public GameObject cardBeingPlayed;
    public bool cardIsPlaying;

    private bool selected;

    private float cardSpacingX = 1.7f;


    //NEW STUFF FOR CONTROLSV2
    public void ShiftCards(float changeX)
    {
        selected = false;

        int checkIndex;
        //change to upper int
        if (changeX > 0)
            checkIndex = Mathf.CeilToInt(changeX);
        else checkIndex = Mathf.RoundToInt(changeX);

        if (startPoint + checkIndex >= cardsInHand.Count)
            selectedIndex = cardsInHand.Count-1;
        else if (startPoint + checkIndex < 0)
            selectedIndex = 0;
        else selectedIndex = startPoint + checkIndex;

        UpdatePositions(changeX);
        UpdateCardLayers();
    }

    private void UpdatePositions(float changeX)
    {

        float middlePos = changeX + startPoint;
        //Debug.Log("middlePos: " + middlePos);

        if (cardsInHand.Count == 0)
            return;

        float spacingX = cardSpacingX;
        float spacingY = 0.1f;
        float firstPos = 0f - spacingX * (middlePos);

        for (int i = 0; i < cardsInHand.Count; i++)
        {
            float posX = firstPos + i * spacingX;
            float spaceFromSelected = Mathf.Abs(i - middlePos);
            float posY = -spacingY * spaceFromSelected;
            Vector2 newPos = new Vector2(posX, -2.2f + posY);
            //Quaternion newRot = Quaternion.LookRotation(Vector3.forward, new Vector3(0, 0, 10f * (i - middlePos)));
            Quaternion rot = Quaternion.AngleAxis((-5f * (i - middlePos)), Vector3.forward);
            cardsInHand[i].transform.DOMove(newPos, 0.1f); //past time was 0.4
            cardsInHand[i].transform.DOLocalRotateQuaternion(rot, 0.2f);
        }
    }

    public void SnapIntoPosition()
    {
        float middlePos = selectedIndex;

        if (cardsInHand.Count == 0)
            return;

        float spacingX = cardSpacingX;
        float spacingY = 0.1f;
        float firstPos = 0f - spacingX * middlePos;

        for (int i = 0; i < cardsInHand.Count; i++)
        {
            float posX = firstPos + i * spacingX;
            float spaceFromSelected = Mathf.Abs(i - middlePos);
            float posY = -spacingY * spaceFromSelected;
            Vector2 newPos = new Vector2(posX, -2.2f + posY);
            //Quaternion newRot = Quaternion.LookRotation(Vector3.forward, new Vector3(0, 0, 10f * (i - middlePos)));
            Quaternion rot = Quaternion.AngleAxis((-5f * (i - middlePos)), Vector3.forward);
            cardsInHand[i].transform.DOMove(newPos, 0.2f); //past time was 0.4
            cardsInHand[i].transform.DOLocalRotateQuaternion(rot, 0.2f);
        }

        selected = true;
        //Debug.Log("Selected index: " + selectedIndex);
    }


    //END



    private void Start()
    {
        SelectInitialCard();
    }


    #region MULTI SELECTED CARDS - not fully functional yet.
    public void SelectCard()
    {
        if (cardsInHand.Count == 0 || cardsSelected.Count == 2)
            return;

        cardsInHand[selectedIndex].GetComponent<SpriteRenderer>().sortingOrder = 2;
        cardsInHand[selectedIndex].transform.DOMove(cardPlayPosition, 0.4f);
        //om inget card �r being played, play selected card,
        if (cardBeingPlayed == null)
        {
            PlayCard();
        }
        else //annars ska selected card bli skickat till cardBeingPlayed f�r att bli p�verkat
        {
            //sortera kort som �r uppe
            //skicka selected card att anv�ndas /buffa/k�kas/etc till played card 
            cardsSelected.Add(cardBeingPlayed);
            cardsSelected.Add(cardsInHand[selectedIndex]);
            cardsInHand.RemoveAt(selectedIndex);
            UpdateSelectedCardPositions();

            //temp f�r att testa, DeSelectCard b�r nog triggas n�gonannan stans ifr�n
            Invoke("DeSelectCard", 2f);
            Invoke("RemoveCard", 2.5f);
        }
    }
    private void UpdateSelectedCardPositions()
    {
        if (cardsSelected.Count == 0)
            return;
        float spacingX = 2f;
        float firstPos = 0f - spacingX;

        for (int i = 0; i < cardsSelected.Count; i++)
        {
            float posX = firstPos + i * spacingX;
            Vector2 newPos = new Vector2(posX, cardPlayPosition.y);

            cardsSelected[i].transform.DOMove(newPos, 0.4f);

        }
        UpdateCardLayers();
    }
    private void DeSelectCard()
    {
        //return to og position
        cardsInHand.Add(cardsSelected[1]);
        cardsSelected.RemoveAt(1);
        UpdateCardPositions();
    }
    public void PlayCard() 
    {
        if (cardsInHand.Count == 0)
            return;

        //this method should probably be in the card script instead?
        //card triggering to destroy itself after having played its animation
        //and done it's actions
        cardBeingPlayed = cardsInHand[selectedIndex];
        //Invoke("RemoveCard",0.5f);

        cardsInHand.RemoveAt(selectedIndex);


        UpdateSelectedIndex();
        UpdateCardPositions();

        if(cardsInHand.Count == 0)
            DrawNewHand();
    }
    #endregion
    public void OldPlayCard()
    {
        if (cardsInHand.Count == 0)
            return;

        GameObject card = cardsInHand[selectedIndex];

        if (!gameBehaviour.CheckMana(card.GetComponent<CardObject>().GetCost()))
        {
            Vector2 oldPos = card.transform.position;
            card.transform.DOMove(cardPlayErrorPosition, 0.2f);
            card.transform.DOMove(oldPos, 0.4f);
            //error sound?
            return;
        }


        card.GetComponent<Canvas>().sortingOrder = 2;
        //card.GetComponent<SpriteRenderer>().sortingOrder = 2; //previously used for old card type
        card.transform.DOMove(cardPlayPosition, 0.4f);
        card.GetComponent<CardObject>().Play();

        //this method should probably be in the card script instead?
        //card triggering to destroy itself after having played its animation
        //and done it's actions
        cardBeingPlayed = card;
        cardIsPlaying = true;
        //Invoke("RemoveCard",0.5f);

        cardsInHand.RemoveAt(selectedIndex);


        UpdateSelectedIndex();
        UpdateCardPositions();

        /*
        if (cardsInHand.Count == 0)
            DrawNewHand();
        */
    }

    private void RemoveCard()
    {
        //cardsSelected.RemoveAt(0);
        cardIsPlaying = false;
        Destroy(cardBeingPlayed);
    }

    private void UpdateSelectedIndex()
    {
        if(selectedIndex > 0)
        --selectedIndex;
    }

    public GameObject DrawCard() 
    {

        //check if can draw card
        //instantiate new card
        GameObject cardObj = Instantiate(cardPrefab);
        
        //SpriteRenderer r = card.GetComponent<SpriteRenderer>();
        //r.sprite = testCard.Sprite;
        //r.color = UnityEngine.Random.ColorHSV();

        //add to cardsInHand
        cardsInHand.Add(cardObj);
        //update positions
        UpdateCardPositions();

        return cardObj;
    }

    public void DrawCard(Card card)
    {
        GameObject cardObj = DrawCard();
        cardObj.GetComponent<CardObject>().SetCard(card);
    }

    public void DrawNewHand() 
    {
        for(int i = 0; i < amountToDraw; i++)
        {
            DrawCard();
        }
    }

    public IEnumerator DrawNewHandNew()
    {
        for (int i = 0; i < amountToDraw; i++)
        {
            DrawCard();
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void EmptyHand()
    {
        selectedIndex = 0;
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            Destroy(cardsInHand[i].gameObject);
        }
        cardsInHand.RemoveRange(0, cardsInHand.Count);
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
        //this is method called from Controls wihout parameters
        UpdateCardPositions(selectedIndex);
    }
    private void UpdateCardPositions(float changeX) 
    {
        float middlePos = changeX;// = selectedIndex;

        if (cardsInHand.Count == 0)
            return;
        float spacingX = cardSpacingX;// 0.8f;
        float spacingY = 0.1f;
        float firstPos = 0f - spacingX * middlePos;

        for (int i = 0; i < cardsInHand.Count; i++)
        {
            float posX = firstPos + i * spacingX;
            float spaceFromSelected = Mathf.Abs(i - middlePos);
            float posY = -spacingY * spaceFromSelected;
            Vector2 newPos = new Vector2(posX, -2.2f + posY);
            Quaternion newRot = Quaternion.LookRotation(Vector3.forward, new Vector3(0,0,10f * (i- middlePos)));
            Quaternion rot = Quaternion.AngleAxis((-5f * (i - middlePos)),Vector3.forward);

            cardsInHand[i].transform.DOKill();

            cardsInHand[i].transform.DOMove(newPos, 0.4f);
            cardsInHand[i].transform.DOLocalRotateQuaternion(rot, 0.2f);
        }

        /*
        //l�gg ut kort till v�nster om selected card
        for(int i = 0; i < selectedIndex; i++)
        {
            float posX = firstPos - i * spacing;
            Vector2 newPos = new Vector2(posX, -2f);
            cardsInHand[i].transform.DOMove(newPos, 0.4f);
        }
        for(int i = 0; i < 1; i++)
        {
            //L�gg ut selected card
            float posX = 0;
            Vector2 newPos = new Vector2(posX, -2f);
            cardsInHand[selectedIndex].transform.DOMove(newPos, 0.4f);
        }
        //l�gg ut kort till h�ger om selected card
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
        //v�nstra sidan fr�n selected index
        for(int i = 0; i < selectedIndex; i++)
        {
            cardsInHand[i].GetComponent<Canvas>().sortingOrder = -1 * (selectedIndex - i);
            //cardsInHand[i].GetComponent<SpriteRenderer>().sortingOrder = -1 * (selectedIndex - i);
        }
        //selected index
        cardsInHand[selectedIndex].GetComponent<Canvas>().sortingOrder = 1;
        //cardsInHand[selectedIndex].GetComponent<SpriteRenderer>().sortingOrder = 1;
        //h�gra sidan fr�n selected index
        for (int i = selectedIndex+1; i < cardsInHand.Count;i++)
        {
            cardsInHand[i].GetComponent<Canvas>().sortingOrder = -1 * (i -(selectedIndex) +1);
            //cardsInHand[i].GetComponent<SpriteRenderer>().sortingOrder = -1 * (i - (selectedIndex) + 1);
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

    /*
    private void ShiftCardsByOne()
    {
        GameObject temp = cardsInHand[0];
        cardsInHand[0] = cardsInHand[cardsInHand.Count-1];
        cardsInHand[cardsInHand.Count - 1] = temp;

        
        //for(int i = 0; i < cardsInHand.Count; i++) 
        //{
            ShiftCard(i);
        //}
        
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
        
        //GameObject temp = cardsInHand[0];
        //cardsInHand[0] = cardsInHand[cardsInHand.Count - 1];
        //cardsInHand.RemoveAt(cardsInHand.Count - 1);
        //cardsInHand.Add(temp);
        

        UpdateCardPositions();
    }
*/

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
            //v�nta lite tid, spela animation, och sen
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
            //om �r vid h�gra kanten kortet
            if(selectedIndex == cardsInHand.Count)
            {
                //flytta ett till h�ger (och sortera)
                ScrollRight();
            }
             //Beh�vs inte f�r att List<T> automatiskt sorterar bort fr�n l�gsta v�rdet perhaps?
            else if(false)//selectedIndex == 0)
            {
                //flytta ett till v�nster (och sortera)
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
