using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;

public class CardHand : MonoBehaviour
{
    public static CardHand Instance { get; private set; }

    [SerializeField]
    private GameBehaviour gameBehaviour;

    [SerializeField]
    private
#if UNITY_EDITOR
        new
#endif
        Camera camera;

    [SerializeField]
    private int amountToDraw = 6;

    [SerializeField]
    private int defaultSelectedCardsLimit = 3;
    public int CurrentSelectedCardsLimit { get; set; }

    [Space]
    [SerializeField]
    private AnimationCurve cardMoveCurve;
    [SerializeField]
    private float cardScreenEdgeOffset;
    [SerializeField]
    private float maxCardRotation;
    [SerializeField]
    private Vector2 cardSpacing = new Vector2(1.7f, 0.1f);
    [SerializeField]
    private AnimationCurve cardSizeCurve;
    [SerializeField]
    private float cardSmallestSize = 0.7f;

    [SerializeField]
    private AnimationCurve cardMiniBounceCurve;

    int startSortingOrder;

    int currentIndex;

    private int CurrentIndex
    {
        get { return currentIndex; }
        set 
        {
            float previous = currentIndex;
            currentIndex = value;

            if (currentIndex != previous)
            {
                OnSelectedIndexChanged();
            }

        }
    }

    float cardsYPos = -2.2f;

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
        startPoint = CurrentIndex;
    }

    public bool DoDiscardState => cardsInHand.Count > 0;

    [SerializeField] CardObject cardPrefab;
    //[SerializeField] Card testCard;
    DeckManager deck;
    Vector3 cardPlayPosition = new Vector3(0, 0.5f, 0);
    List<CardObject> cardsInHand = new List<CardObject>();
    //List<CardObject> cardsSelected = new List<CardObject>();

    public CardObject CardBeingPlayed { get; private set; }
    public bool IsPlayingCard { get; private set; }

    [Header("Selecting Multiple Cards")]
    [SerializeField]
    SpriteRenderer selectingCardsOverlayBg;
    [SerializeField]
    TMP_Text selectingCardsText;
    [SerializeField]

    [Space]
    [TextArea(1, 3)]
    string discardStateTextFormat;

    [Space]
    [TextArea(1, 3)]
    [SerializeField]
    string affectCardsTextFormat;
    [TextArea(1, 3)]
    [SerializeField]
    string affectSingleCardTextFormat;
    [TextArea(1, 3)]
    [SerializeField]
    string affectUnlimitedCardsTextFormat;

    private string _affectCardsCustomFormat;

    public bool SelectingCards { get; set; }
    public SelectionState SelectingCardsState { get; private set; }

    public int SelectedCardsCount { get; private set; }
    public HashSet<CardObject> SelectedCards => selectedCards;
    HashSet<CardObject> selectedCards = new HashSet<CardObject>();

    //private bool selected;

    private float screenLeftXPos;
    private float screenRightXPos;
    private Vector2Int oldScreenSize;

    private void Awake()
    {
        Instance = this;

        startSortingOrder = cardPrefab.Canvas.sortingOrder;

        OnChangeResolution();
    }

    private void Update()
    {


        if (oldScreenSize.x == Screen.width && oldScreenSize.y == Screen.height)
        {
            return;
        }






        OnChangeResolution();

    }

    private void OnChangeResolution()
    {
        screenLeftXPos = camera.ViewportToWorldPoint(new Vector2(0, 0)).x;
        screenRightXPos = camera.ViewportToWorldPoint(new Vector2(1, 0)).x;

        oldScreenSize = new Vector2Int(Screen.width, Screen.height);
    }

    private void OnSelectedIndexChanged()
    {
        //här updatera indicators
        //om current index är ett heltal
        if (cardsInHand.Count > 0 && currentIndex % 1 == 0)
        {
            if (!(currentIndex >= cardsInHand.Count) && cardsInHand[currentIndex] != null)
            {
                gameBehaviour.indicatorManager.UpdateIndicators(cardsInHand[currentIndex].Card);
            }
        }
    }


    //NEW STUFF FOR CONTROLSV2
    public void ShiftCards(float changeX)
    {
        //selected = false;

        int checkIndex;
        //change to upper int
        if (changeX > 0)
            checkIndex = Mathf.CeilToInt(changeX);
        else checkIndex = Mathf.RoundToInt(changeX);

        if (startPoint + checkIndex >= cardsInHand.Count)
            CurrentIndex = cardsInHand.Count-1;
        else if (startPoint + checkIndex < 0)
            CurrentIndex = 0;
        else CurrentIndex = startPoint + checkIndex;

        UpdatePositions(changeX);
        UpdateCardLayers();
    }

    private void UpdatePositions(float changeX)
    {
        //float middlePos = changeX + startPoint;
        //Debug.Log("middlePos: " + middlePos);

        if (cardsInHand.Count <= 0)
            return;

        TweenCardPositions(changeX + startPoint, 0.1f, 0.2f, 0.1f);
        /*
        float spacingX = cardSpacingX;
        float spacingY = 0.1f;
        float firstPos = 0f - spacingX * (middlePos);

        for (int i = 0; i < cardsInHand.Count; i++)
        {
            CardObject cardObj = cardsInHand[i];

            if (cardObj == CardBeingPlayed)
            {
                continue;
            }

            float posX = firstPos + i * spacingX;
            float spaceFromSelected = Mathf.Abs(i - middlePos);
            float posY = -spacingY * spaceFromSelected;
            Vector2 newPos = new Vector2(posX, cardsYPos + posY);
            //Quaternion newRot = Quaternion.LookRotation(Vector3.forward, new Vector3(0, 0, 10f * (i - middlePos)));
            Quaternion rot = Quaternion.AngleAxis((-5f * (i - middlePos)), Vector3.forward);

            cardObj.transform.DOKill();
            cardObj.transform.DOMove(newPos, 0.1f); //past time was 0.4
            cardObj.transform.DOLocalRotateQuaternion(rot, 0.2f);
        }
        */
    }

    public void SnapIntoPosition()
    {
        //float middlePos = currentIndex;

        if (cardsInHand.Count <= 0)
            return;

        TweenCardPositions(CurrentIndex, 0.2f, 0.2f, 0.2f);

        /*
        float spacingX = cardSpacingX;
        float spacingY = 0.1f;
        float firstPos = 0f - spacingX * middlePos;

        for (int i = 0; i < cardsInHand.Count; i++)
        {
            CardObject cardObj = cardsInHand[i];

            if (cardObj == CardBeingPlayed)
            {
                continue;
            }

            float posX = firstPos + i * spacingX;
            float spaceFromSelected = Mathf.Abs(i - middlePos);
            float posY = -spacingY * spaceFromSelected;
            Vector2 newPos = new Vector2(posX, cardsYPos + posY);
            //Quaternion newRot = Quaternion.LookRotation(Vector3.forward, new Vector3(0, 0, 10f * (i - middlePos)));
            Quaternion rot = Quaternion.AngleAxis((-5f * (i - middlePos)), Vector3.forward);
            cardObj.transform.DOKill();
            cardObj.transform.DOMove(newPos, 0.2f); //past time was 0.4
            cardObj.transform.DOLocalRotateQuaternion(rot, 0.2f);
        }

        //selected = true;
        //Debug.Log("Selected index: " + selectedIndex);
        */
    }

    private void TweenCardPositions(float middlePos, float posDuration, float rotDuration, float scaleDuration)
    {
        int count = cardsInHand.Count;

        for (int i = 0; i < count; i++)
        {
            CardObject cardObj = cardsInHand[i];

            if (cardObj == CardBeingPlayed)
            {
                continue;
            }

            TweenCardPosition(cardObj, i, middlePos, posDuration, rotDuration, scaleDuration);
        }
    }

    private void TweenCardPosition(CardObject cardObj, int i, float middlePos, float posDuration = 0, float rotDuration = 0, float scaleDuration = 0)
    {
        float SampleCurve(float t, AnimationCurve curve)
        {
            t *= 2;

            if (t > 1)
            {
                t = 1 - (t - 1);
            }

            return curve.Evaluate(t);
        }

        float posX = (-cardSpacing.x * middlePos) + i * cardSpacing.x;

        float leftPos = screenLeftXPos - cardScreenEdgeOffset;
        float rightPos = screenRightXPos + cardScreenEdgeOffset;

        float t = Mathf.InverseLerp(leftPos, rightPos, posX);

        float zeroPos = t > 0.5f ? rightPos : leftPos;

        posX = Mathf.Lerp(zeroPos, (leftPos + rightPos) / 2, SampleCurve(t, cardMoveCurve));

        float spaceFromSelected = Mathf.Abs(i - middlePos);
        float posY = -cardSpacing.y * spaceFromSelected;
        Vector2 newPos = new Vector2(posX, cardsYPos + posY);

        //Quaternion newRot = Quaternion.LookRotation(Vector3.forward, new Vector3(0, 0, 10f * (i - middlePos)));
        //Quaternion rot = Quaternion.AngleAxis((-5f * (i - middlePos)), Vector3.forward);
        float newRot = Mathf.Lerp(maxCardRotation, -maxCardRotation, t);

        float newScale = Mathf.Lerp(cardSmallestSize, 1, SampleCurve(t, cardSizeCurve));

        cardObj.TweenTransformInHand(newPos, newRot, newScale, posDuration, rotDuration, scaleDuration);
        /*
        cardObj.transform.DOKill();
        cardObj.transform.DOMove(newPos, moveDuration);
        cardObj.transform.DOLocalRotateQuaternion(rot, rotateDuration);
        cardObj.transform.DOScale(newScale, scaleDuration);
        */
    }

    //END

    private void Start()
    {
        deck = DeckManager.Instance;

        SelectInitialCard();
    }

    /*
    #region MULTI SELECTED CARDS - not fully functional yet.
    public void SelectCard()
    {
        if (cardsInHand.Count <= 0 || cardsSelected.Count >= 2)
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
        if (cardsSelected.Count <= 0)
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
        if (cardsInHand.Count <= 0)
            return;

        if (InDiscardState)
            return;

        //this method should probably be in the card script instead?
        //card triggering to destroy itself after having played its animation
        //and done it's actions
        cardBeingPlayed = cardsInHand[selectedIndex];
        //Invoke("RemoveCard",0.5f);

        cardsInHand.RemoveAt(selectedIndex);


        UpdateSelectedIndex();
        UpdateCardPositions();

        /*
        if(cardsInHand.Count <= 0)
            DrawNewHand();
        * /
    }
    #endregion
    */

    public void UseCurrentCard()
    {
        int count = cardsInHand.Count;
        if (count <= 0 || CurrentIndex < 0 || CurrentIndex >= count)
            return;

        CardObject cardObj = cardsInHand[CurrentIndex];

        if (SelectingCards)
        {
            UseCardDuringSelection(cardObj);
            DoCardMiniBounce(cardObj);
            return;
        }

        int cost = cardObj.GetCost();

        if (!gameBehaviour.CheckMana(cost) || IsPlayingCard)
        {
            //error sound?

            DoCardMiniBounce(cardObj);
            return;
        }

        // remove mana
        gameBehaviour.LoseMana(cost);

        CardBeingPlayed = cardObj;
        IsPlayingCard = true;

        cardsInHand.RemoveAt(CurrentIndex);

        cardObj.Canvas.sortingOrder = startSortingOrder + count + 10;
        cardObj.CardVisuals.CanvasGroup.blocksRaycasts = false;

        UpdateCurrentIndex();
        UpdateCardPositions();

        cardObj.OnCardPressed -= OnCardPressed;
        cardObj.TweenTransformInHand(cardPlayPosition, 0, 1, 1, 1, 1, Ease.OutExpo);

        StartCoroutine(DelayPlayCard(cardObj));
    }

    private IEnumerator DelayPlayCard(CardObject cardObj)
    {
        yield return new WaitForSeconds(0.15f);

        cardObj.Dissolve();

        yield return new WaitForSeconds(0.15f);

        cardObj.Play();
    }

    /*
    public void PlayCard(CardObject cardObj)
    {
        cardObj.Play();
        //Invoke("RemoveCard",0.5f);

        //UpdateSelectedIndex();
        //UpdateCardPositions();

        /*
        if (cardsInHand.Count == 0)
            DrawNewHand();
        * /
    }
    */

    private void MoveCardToCenter(CardObject cardObj)
    {
        //move to center this card
        for(int i = 0; i < cardsInHand.Count; i++)
        {
            if (cardsInHand[i] == cardObj)
            {
                CurrentIndex = i;
                UpdateCardPositions();
            }
        }
    }

    // MAKE EM DO A LIL CUTE BOUNCE
    private void DoCardMiniBounce(CardObject cardObj)
    {
        cardObj.TweenOffsetY(0.2f, 0.2f).SetEase(cardMiniBounceCurve).onComplete = () =>
        {
            cardObj.TweenOffsetY(0, 0.1f);
        };

        /*
        cardObj.transform.DOKill();
        cardObj.transform.DOMoveY(cardMiniBouncePosition, 0.2f);
        cardObj.transform.DOMoveY(cardObj.StartYPos, 0.4f).SetDelay(0.015f);
        */
    }

    public void DiscardCard(CardObject cardObj) => DiscardCard(cardObj.Card);

    public void DiscardCard(Card card)
    {
        deck.Discard(card);
    }

    private void UpdateCurrentIndex()
    {
        if(CurrentIndex > 0)
            --CurrentIndex;
    }

    private void UseCardDuringSelection(CardObject cardObj)
    {
        if (!cardObj.Selectable)
        {
            return;
        }

        void Select(CardObject cardObj)
        {
            selectedCards.Add(cardObj);
            SelectedCardsCount++;

            cardObj.ToggleCheckmark(true);
            cardObj.ToggleDarkOverlay(true);
        }

        void Unselect(CardObject cardObj)
        {
            selectedCards.Remove(cardObj);
            SelectedCardsCount--;

            cardObj.ToggleCheckmark(false);
            cardObj.ToggleDarkOverlay(false);
        }

        if (selectedCards.Contains(cardObj))
        {
            Unselect(cardObj);

            OnUpdateSelectedCards();

            return;
        }

        if (CurrentSelectedCardsLimit > 1 && selectedCards.Count >= CurrentSelectedCardsLimit)
        {
            return;
        }

        if (CurrentSelectedCardsLimit == 1)
        {
            foreach (CardObject card in selectedCards)
            {
                card.ToggleCheckmark(false);
                card.ToggleDarkOverlay(false);
            }

            selectedCards.Clear();
            SelectedCardsCount = 0;
        }

        Select(cardObj);

        OnUpdateSelectedCards();
    }

    private void OnUpdateSelectedCards()
    {
        UpdateSelectingCardsText();
    }

    private void UpdateSelectingCardsText(int? overrideCount = null)
    {
        int count = overrideCount.HasValue ? overrideCount.Value : selectedCards.Count;
        string format;

        switch (SelectingCardsState)
        {
            case SelectionState.Discard:
                format = discardStateTextFormat;
                break;

            case SelectionState.AffectCards:
                if (!string.IsNullOrEmpty(_affectCardsCustomFormat))
                {
                    format = _affectCardsCustomFormat;
                }
                else if (CurrentSelectedCardsLimit == 1)
                {
                    format = affectSingleCardTextFormat;
                }
                else if (CurrentSelectedCardsLimit > 0)
                {
                    format = affectCardsTextFormat;
                }
                else
                {
                    format = affectUnlimitedCardsTextFormat;
                }
                break;

            default:
                format = null;
                break;
        }

        selectingCardsText.text = string.Format(format, count, CurrentSelectedCardsLimit == 0 ? "infinite" : CurrentSelectedCardsLimit);
    }

    public CardObject DrawCard(bool updateCurrentIndex = true) => DrawCard(deck.DrawNext());

    public CardObject DrawCard(Card card, bool updateCurrentIndex = true)
    {
        //check if can draw card
        //if(!deck.CanDrawNext()) return null;

        //instantiate new card
        CardObject cardObj = Instantiate(cardPrefab);
        cardObj.TimeCreated = Time.time;

        cardObj.CardHand = this;

        cardObj.StartYPos = cardsYPos;
        cardObj.OnCardPressed += OnCardPressed;

        cardObj.Initialize(card);

        //SpriteRenderer r = card.GetComponent<SpriteRenderer>();
        //r.sprite = testCard.Sprite;
        //r.color = UnityEngine.Random.ColorHSV();

        // Insert the CardObject into the cardsInHand list in the correct order (as determined by the card sorter)
        int count = cardsInHand.Count;
        int index;

        CardObject selectedCard = count > 0 ? cardsInHand[CurrentIndex] : null;
        
        // Stolen from: https://stackoverflow.com/questions/12172162/how-to-insert-item-into-list-in-order
        if (count <= 0 || CardSorter.Instance.Compare(cardsInHand[count - 1], cardObj) <= 0)
        {
            cardsInHand.Add(cardObj);
            index = count - 1;
        }
        else if (CardSorter.Instance.Compare(cardsInHand[0], cardObj) >= 0)
        {
            cardsInHand.Insert(0, cardObj);
            index = 0;
        }
        else
        {
            index = cardsInHand.BinarySearch(cardObj, CardSorter.Instance);

            if (index < 0)
                index = ~index;

            cardsInHand.Insert(index, cardObj);
        }

        if (updateCurrentIndex && selectedCard != null && cardsInHand[CurrentIndex] != selectedCard)
        {
            CurrentIndex++;
        }

        UpdateCardPositions();

        return cardObj;
    }

    /*
    public void DrawCard(Card card)
    {
        CardObject cardObj = DrawCard();

#if UNITY_EDITOR
        if (!cardObj.Debugging)
        {
#endif
            cardObj.Initialize(card);
#if UNITY_EDITOR
        }
#endif
    }
    */

    public IEnumerator DrawNewHand()
    {
        int count = cardsInHand.Count;

        if (count <= 0)
        {
            CurrentIndex = 0;
        }

        // Draw cards in sorted order
        List<Card> cardsToDraw = new();

        for (int i = count; i < amountToDraw; i++)
        {
            if (!deck.CanDrawNext())
            {
                break;
            }

            cardsToDraw.Add(deck.DrawNext());
        }

        cardsToDraw.Sort(CardSorter.Instance);

        foreach (Card card in cardsToDraw)
        {
            DrawCard(card, count > 0);

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void SortHand()
    {
        cardsInHand.Sort(CardSorter.Instance);
    }

    // THIS IS CALLED WHEN THE CARD IS TAPPED ON (NOT SWIPED UP)
    private void OnCardPressed(CardObject cardObj)
    {
        if (cardObj == CardBeingPlayed)
        {
            return;
        }

        /* TODO: Make this work
        int index = cardsInHand.IndexOf(cardObj);

        if (index >= 0)
        {
            currentIndex = index;
            SnapIntoPosition();
        }
        */

        DoCardMiniBounce(cardObj);

        MoveCardToCenter(cardObj);

        if (SelectingCards)
        {
            UseCardDuringSelection(cardObj);
            return;
        }
    }

    public void EmptyHand()
    {
        CurrentIndex = 0;

        for (int i = cardsInHand.Count - 1; i >= 0; i--)
        {
            CardObject cardObj = cardsInHand[i];

            if (SelectingCards && SelectingCardsState == SelectionState.Discard && selectedCards.Contains(cardObj))
            {
                continue;
            }

            cardObj.Canvas.sortingOrder -= startSortingOrder;
            cardObj.OnCardPressed -= OnCardPressed;

            DiscardCard(cardObj);
            cardObj.Dissolve();
            cardObj.Destroy();

            cardObj.TweenOffsetY(-15, 1).SetEase(Ease.InCirc);

            cardsInHand.RemoveAt(i);
        }
    }

    private void SelectInitialCard()
    {
        //selected index = Middle position of cards in hand. If middle is below 1, set to 1
        if ((cardsInHand.Count / 2) < 1)
            CurrentIndex = 0;
        else
            CurrentIndex = Mathf.RoundToInt(cardsInHand.Count / 2);
    }

    private void UpdateCardPositions()
    {
        //this is method called from Controls wihout parameters
        UpdateCardPositions(CurrentIndex);
    }

    private void UpdateCardPositions(float changeX) 
    {
        float middlePos = changeX;// = selectedIndex;

        if (cardsInHand.Count == 0)
            return;

        TweenCardPositions(changeX, 0.4f, 0.2f, 0.4f);

        /*
        float spacingX = cardSpacingX;// 0.8f;
        float spacingY = 0.1f;
        float firstPos = 0f - spacingX * middlePos;

        for (int i = 0; i < cardsInHand.Count; i++)
        {
            CardObject cardObj = cardsInHand[i];

            if (cardObj == CardBeingPlayed)
            {
                continue;
            }

            float posX = firstPos + i * spacingX;
            float spaceFromSelected = Mathf.Abs(i - middlePos);
            float posY = -spacingY * spaceFromSelected;
            Vector2 newPos = new Vector2(posX, cardsYPos + posY);
            Quaternion newRot = Quaternion.LookRotation(Vector3.forward, new Vector3(0,0,10f * (i- middlePos)));
            Quaternion rot = Quaternion.AngleAxis((-5f * (i - middlePos)),Vector3.forward);

            cardObj.transform.DOKill();
            cardObj.transform.DOMove(newPos, 0.4f);
            cardObj.transform.DOLocalRotateQuaternion(rot, 0.2f);
        }
        */

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
        for (int i = 0; i < CurrentIndex; i++)
        {
            cardsInHand[i].Canvas.sortingOrder = -1 * (CurrentIndex - i);
            //cardsInHand[i].GetComponent<SpriteRenderer>().sortingOrder = -1 * (selectedIndex - i);
        }
        //selected index
        cardsInHand[CurrentIndex].Canvas.sortingOrder = 1;
        //cardsInHand[selectedIndex].GetComponent<SpriteRenderer>().sortingOrder = 1;
        //h�gra sidan fr�n selected index
        for (int i = CurrentIndex+1; i < cardsInHand.Count;i++)
        {
            cardsInHand[i].Canvas.sortingOrder = -1 * (i -(CurrentIndex) + 1);
            //cardsInHand[i].GetComponent<SpriteRenderer>().sortingOrder = -1 * (i - (selectedIndex) + 1);
        }

        int offset = startSortingOrder + cardsInHand.Count;

        foreach (CardObject cardObj in cardsInHand)
        {
            cardObj.Canvas.sortingOrder += offset;
        }
    }

    public void ShiftAllRight()
    {
        if (!CheckIfCardNextTo(-1))
            return;

        --CurrentIndex;
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

        ++CurrentIndex;
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
        if (CurrentIndex + direction >= cardsInHand.Count || CurrentIndex + direction < 0)
        { return false; }
        return true;
    }

    public void OnStartSelectingCards(SelectionState state, int? selectedCardsLimit = null, string textFormat = null, CardFilter filter = null)
    {
        SelectingCards = true;

        SelectingCardsState = state;

        selectedCards.Clear();
        SelectedCardsCount = 0;

        foreach (CardObject cardObj in cardsInHand)
        {
            bool disable = false;

            if (filter != null)
            {
                CardFilterResult result = filter.Invoke(cardObj);

                if (result != null && result.Failed)
                {
                    disable = true;

                    cardObj.SetDisabledText(result.FailMessage);
                }
            }

            cardObj.ToggleSelectable(!disable);
            cardObj.ToggleDarkOverlay(disable);

            cardObj.ToggleCheckmark(false);
        }

        selectingCardsOverlayBg.DOKill();
        selectingCardsOverlayBg.DOFade(0.7f, 0.5f);

        selectingCardsText.DOKill();
        selectingCardsText.DOFade(1, 0.5f);

        CurrentSelectedCardsLimit = selectedCardsLimit.HasValue ? selectedCardsLimit.Value : defaultSelectedCardsLimit;

        _affectCardsCustomFormat = textFormat;

        UpdateSelectingCardsText();
    }

    public void OnExitSelectingCards()
    {
        SelectingCards = false;

        selectedCards.Clear();
        SelectedCardsCount = 0;

        foreach (CardObject cardObj in cardsInHand)
        {
            cardObj.ToggleSelectable(true);
            cardObj.ToggleDarkOverlay(false);

            cardObj.ToggleCheckmark(false);
        }

        selectingCardsOverlayBg.DOKill();
        selectingCardsOverlayBg.DOFade(0, 0.5f);

        selectingCardsText.DOKill();
        selectingCardsText.DOFade(0, 0.5f);
    }

    public void OnFinishPlayingCard()
    {
        CardBeingPlayed = null;
        IsPlayingCard = false;
    }

    public enum SelectionState
    {
        Discard,
        AffectCards,
    }

    /*
    public void OnEnterSelectAdditionalCardState()
    {
        choosingCardsToAffect = true;
        cardsToAffect.Clear();

        maxAmountToKeepAfterDiscarding = 1; // cardBeingPlayed.amountToKeep;


        InDiscardState = true;

        cardsToKeepAfterDiscard.Clear();

        foreach (CardObject cardObj in cardsInHand)
        {
            cardObj.CheckmarkDisappear();
        }

        overlayBg.DOKill();
        overlayBg.DOFade(0.7f, 0.5f);

        discardStateText.DOKill();
        discardStateText.DOFade(1, 0.5f);

        discardStateText.text = "Choose cards to affect.";// string.Format(discardStateTextFormat, 0, maxAmountToKeepAfterDiscarding);
    }

    public void OnExitSelectAdditionalCardState()
    {
        InDiscardState = false;

        cardsToKeepAfterDiscard.Clear();

        choosingCardsToAffect = false;
        cardsToAffect.Clear();

        foreach (CardObject cardObj in cardsInHand)
        {
            cardObj.CheckmarkDisappear();
        }

        overlayBg.DOKill();
        overlayBg.DOFade(0, 0.5f);

        discardStateText.DOKill();
        discardStateText.DOFade(0, 0.5f);

        maxAmountToKeepAfterDiscarding = 3; //resetting it to what is needed for discard thingy
    }
    */

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
