using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SingletonMode(true)]
public class DeckManager : Singleton<DeckManager>
{
    public CardDeck DefaultStarterDeck => defaultStarterDeck;

    [SerializeField] private CardDeck defaultStarterDeck;

    public List<Card> deck {get; private set;}
    public Stack<Card> drawPile { get; private set; }
    public LinkedList<Card> discardPile { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        discardPile = new LinkedList<Card>();
        deck = new List<Card>();

        InitializeDeck(defaultStarterDeck);
    }

    public void InitializeDeck(IEnumerable<Card> startingDeck)
    {
        deck.Clear();
        deck.AddRange(startingDeck);

        ResetDeck();
    }

    public void ResetDeck()
    {
        drawPile = new Stack<Card>(deck.Count);
        List<Card> tempCardList = new List<Card>(deck);
        ShuffleDrawFromList(tempCardList);
        discardPile.Clear();
    }

    public void AddCardToDeck(Card card, int copies = 1)
    {
        for (int i = 0; i < copies; i++)
        {
            deck.Add(card);
        }
    }

    public void RemoveCardFromDeck(Card card)
    {
        deck.RemoveAll((match) => match == card);
    }

    public int CardCount(Card card)
    {
        return deck.Count((match) => match == card);
    }

    public bool CanDrawNext()
    {
        return drawPile != null && !(drawPile.Count == 0 && discardPile.Count == 0);
    }

    public Card DrawNext()
    {
        if (drawPile.Count <= 0)
        {
            ReshuffleDeck();
        }

        return drawPile.Pop();
    }

    public Card DrawNextWithTag(CardTag tag)
    {
        int count = drawPile.Count;

        if (count <= 0)
        {
            ReshuffleDeck();
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

            card = drawPile.Pop();

            tries++;

            hasTag = card.HasTag(tag);
        }
        while (!hasTag && tries <= count);

        void ReaddCardsToDeck()
        {
            foreach (Card card in tempPile)
            {
                drawPile.Push(card);
            }
        }

        if (hasTag)
        {
            ReaddCardsToDeck();
            return card;
        }

        if (card != null)
        {
            tempPile.Push(card);
        }

        ReaddCardsToDeck();

        return null;
    }

    public void Discard(Card card)
    {
        discardPile.AddLast(value: card);
    }

    public void InsertInDrawRandom(Card card)
    {
        int randomIndex = Random.Range(0, drawPile.Count);

        LinkedList<Card> drawPileHead = new LinkedList<Card>();

        for(int i = 0; i < randomIndex; i++)
        {
            drawPileHead.AddFirst(drawPile.Pop());
        }

        drawPile.Push(card);

        foreach(Card c in drawPileHead)
        {
            drawPile.Push(c);
        }
    }

    private void ReshuffleDeck()
    {
        List<Card> tempCardList = new List<Card>(discardPile);
        ShuffleDrawFromList(tempCardList);
        discardPile.Clear();
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
            drawPile.Push(card);
        }
    }
}
