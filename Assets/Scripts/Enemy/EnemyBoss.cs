using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class EnemyBoss : MonoBehaviour
{
    [SerializeField] private int bossHealth;
    [SerializeField] private int maxMana;
    [SerializeField] List<Card> cardsAvailable = new List<Card>();

    private int currentMana;

    public void StartTurn()
    {
        currentMana = maxMana;
        PlayCards();
    }

    private void PlayCards()
    {
        while (true)
        {
            if(currentMana <= 0)
            {
                break;
            }

            Card currentCard = PickRandomCard();
            /*
             * Add code to activate the selected card
             */
        }
        EndTurn();
    }
    private Card PickRandomCard()
    {
        /*
         * Should add here to check the mana cost of the cards
         */
        int index = Random.Range(0, cardsAvailable.Count);
            
        
        return cardsAvailable[index];
    }

    private void EndTurn()
    {
        /*
         * Code to end turn here
         */
    }
}
