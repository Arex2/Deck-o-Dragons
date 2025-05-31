using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDeck", menuName = "Cards/Create New Card Deck", order = 20)]
public class CardDeck : ScriptableObject, IEnumerable<Card>
{
    [SerializeField] private CardCopyData[] entries;

    public IEnumerator<Card> GetEnumerator()
    {
        foreach (CardCopyData cardCopyData in entries)
        {
            foreach (Card card in cardCopyData)
            {
                yield return card;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
