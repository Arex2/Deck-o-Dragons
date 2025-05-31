using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[SingletonMode(true)]
public class DeckManager : Singleton<DeckManager>
{
    public CardDeck DefaultStarterDeck => defaultStarterDeck;

    [SerializeField] private CardDeck defaultStarterDeck;

    public List<Card> Deck {get; private set;}
    public Stack<Card> DrawPile { get; private set; }
    public LinkedList<Card> DiscardPile { get; private set; }

    public static Action OnUpdateDrawPile { get; set; }
    public static Action OnUpdateDiscardPile { get; set; }
    public static Action OnUpdateDeck { get; set; }

    private List<Card> BlackListCards;

    protected override void Awake()
    {
        base.Awake();

        DiscardPile = new LinkedList<Card>();
        Deck = new List<Card>();
        DrawPile = new Stack<Card>();
        BlackListCards = new List<Card>();

        InitializeDeck(defaultStarterDeck);
    }

    public void InitializeDeck(IEnumerable<Card> startingDeck)
    {
        Deck.Clear();
        Deck.AddRange(startingDeck);

        ResetDeck();
    }

    public void ResetDeck()
    {
        DrawPile.Clear();
        ShuffleDrawFromList(new List<Card>(Deck.Except(BlackListCards)));
        DiscardPile.Clear();

        OnUpdateDrawPile?.Invoke();
        OnUpdateDiscardPile?.Invoke();

        OnUpdateDeck?.Invoke();
    }

    public void AddCardToDeck(Card card, int copies = 1)
    {
        for (int i = 0; i < copies; i++)
        {
            Deck.Add(card);
        }

        OnUpdateDeck?.Invoke();
    }

    public void RemoveCardFromDeck(Card card)
    {
        Deck.RemoveAll((match) => match == card);

        OnUpdateDeck?.Invoke();
    }

    public bool HasCardInDeck(Card card)
    {
        return Deck.Contains(card);
    }

    public int CardCount(Card card)
    {
        return Deck.Count((match) => match == card);
    }

    public bool CanDrawNext()
    {
        return !(DrawPile.Count == 0 && DiscardPile.Count == 0);
    }

    public Card DrawNext()
    {
        if (DrawPile.Count <= 0)
        {
            ResetDeck();
        }

        Card card = DrawPile.Pop();

        OnUpdateDrawPile?.Invoke();

        return card;
    }

    public Card DrawNextWithTag(CardTag tag)
    {
        int count = DrawPile.Count;

        if (count <= 0)
        {
            ResetDeck();
        }

        Stack<Card> tempPile = new Stack<Card>();

        Card card = null;
        int tries = 0;

        bool hasTag = false;

        do
        {
            if (card != null)
            {
                tempPile.Push(card);
            }

            card = DrawPile.Pop();

            tries++;

            hasTag = card.HasTag(tag);
        }
        while (!hasTag && tries <= count);

        void ReAddCardsToDrawPile()
        {
            foreach (Card card in tempPile)
            {
                DrawPile.Push(card);
            }
        }

        if (hasTag)
        {
            ReAddCardsToDrawPile();

            OnUpdateDrawPile?.Invoke();
            return card;
        }

        if (card != null)
        {
            tempPile.Push(card);
        }

        ReAddCardsToDrawPile();

        return null;
    }

    public void Discard(Card card)
    {
        DiscardPile.AddLast(card);

        OnUpdateDiscardPile?.Invoke();
    }

    public void InsertInDrawRandom(Card card)
    {
        int randomIndex = Random.Range(0, DrawPile.Count);

        LinkedList<Card> drawPileHead = new LinkedList<Card>();

        for(int i = 0; i < randomIndex; i++)
        {
            drawPileHead.AddFirst(DrawPile.Pop());
        }

        DrawPile.Push(card);

        foreach(Card c in drawPileHead)
        {
            DrawPile.Push(c);
        }
    }

    public void AddToBlackList(Card card)
    {
        BlackListCards.Add(card);
    }

    public void RemoveFromBlackList(Card card)
    {
        if(BlackListCards.Contains(card))
        {
            BlackListCards.Remove(card);
        }
    }

    private void ShuffleDrawFromList(List<Card> cards)
    {
        int count = cards.Count;
        while (count > 1)
        {
            count--;
            int index = Random.Range(0, count + 1);
            Card value = cards[index];
            cards[index] = cards[count];
            cards[count] = value;
        }

        //Make sure this is done from a temporary List
        //cards.OrderBy(card => Random.Range(minInclusive: 1f, maxInclusive: 100f));

        foreach(Card card in cards)
        {
            DrawPile.Push(card);
        }
    }
}
