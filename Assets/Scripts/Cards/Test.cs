using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private Card card;
    [SerializeField] private Target user;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Play();
        }
    }

    public void Play()
    {
        card.Play(user, () => Debug.Log("I'm finish"));
    }
}
