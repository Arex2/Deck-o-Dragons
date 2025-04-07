using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardObject : MonoBehaviour
{
    [SerializeField] private Card card;
    [SerializeField] private Target user;

    private void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = card.Sprite;
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
