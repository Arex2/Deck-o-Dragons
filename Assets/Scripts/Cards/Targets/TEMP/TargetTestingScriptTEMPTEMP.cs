using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: REMOVE
public class TargetTestingScriptTEMPTEMP : MonoBehaviour
{
    [SerializeField] private Card card;
    [SerializeField] private Card otherCard;
    [SerializeField] private Target user;
    [SerializeField] private Target user2;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Play(card, user);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Play(otherCard, user2 == null ? user : user2);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("STARTED turn of everyone");

            foreach (Target target in TargetManager.AllTargets)
            {
                target.OnTurnStart();
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("ENDED turn of everyone");

            foreach (Target target in TargetManager.AllTargets)
            {
                target.OnTurnEnd();
            }
        }
    }

    public void Play(Card card, Target user)
    {
        card.Play(user, OnFinishPlayingCard);
    }

    private void OnFinishPlayingCard()
    {
        Debug.Log("I'm finish");
    }
}
