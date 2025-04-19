using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: REMOVE
public class TargetTestingScriptTEMPTEMP : MonoBehaviour
{
    [SerializeField] private Card card;
    [SerializeField] private Card otherCard;
    [SerializeField] private Target user;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Play(card);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Play(otherCard);
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

    public void Play(Card card)
    {
        card.Play(user, OnFinishPlayingCard);
    }

    private void OnFinishPlayingCard()
    {
        Debug.Log("I'm finish");
    }
}
