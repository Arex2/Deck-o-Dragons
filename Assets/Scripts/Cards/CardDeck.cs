using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "CardDeck", menuName = "Cards/Create New Card Deck", order = 20)]
public class CardDeck : ScriptableObject, IEnumerable<Card>
{
    [SerializeField] private Card[] cards;

    public IEnumerator<Card> GetEnumerator()
    {
        foreach (Card card in cards)
        {
            for (int i = 0; i < card.Copies; i++)
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
