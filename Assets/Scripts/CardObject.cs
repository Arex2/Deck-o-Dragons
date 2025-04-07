using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardObject : MonoBehaviour
{
    [SerializeField] private Card card;
    [SerializeField] private Target user;
    [SerializeField] private Card[] cards;

    private void Start()
    {
        BecomeRandomCard();
        gameObject.GetComponent<SpriteRenderer>().sprite = card.Sprite;
    }

    private void BecomeRandomCard()
    {
        int i = Random.Range(0, cards.Length);
        card = cards[i];
    }

    public int GetCost()
    {
        return card.Cost;
    }

    public void Play()
    {
        card.Play(user, OnFinishPlayingCard);
    }

    private void OnFinishPlayingCard()
    {
        Debug.Log("I'm finish");
        //remove mana from target??

        //destroy itself
        StartCoroutine(DeleteItself());
    }

    IEnumerator DeleteItself()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

}
