using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public List<Card> deck;
    public Stack<Card> drawPile;
    public LinkedList<Card> discardPile;

    public void InitializeDeck(Card[] startingDeck)
    {
        deck.AddRange(startingDeck);
    }

    public void ResetDeck()
    {
        drawPile = new Stack<Card>(deck.Count);
        List<Card> tempCardList = new List<Card>(deck);
        ShuffleDrawFromList(tempCardList);
        discardPile.Clear();
    }

    public Card DrawNext()
    {
        if(drawPile.Count == 0)
        {
            ReshuffleDeckFromDiscard();
        }

        return drawPile.Pop();
    }

    public void Discard(Card card)
    {
        discardPile.AddLast(card);
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

    private void ReshuffleDeckFromDiscard()
    {
        List<Card> tempCardList = new List<Card>(drawPile);
        ShuffleDrawFromList(tempCardList);
        discardPile.Clear();
    }

    private void ShuffleDrawFromList(List<Card> cards)
    {
        //Make sure this is done from a temporary List
        cards.OrderBy(card => Random.Range(minInclusive: 1f, maxInclusive: 100f));
        foreach(Card card in cards)
        {
            drawPile.Push(card);
        }
    }
}
