using System.Collections;
using UnityEngine;

/// <summary>
/// The script that handles loading every <see cref="Card"/> object.
/// </summary>
// Script by Ruben
[SingletonMode(true)]
public class CardManager : Singleton<CardManager>
{
    public static Card[] AllCards { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        AllCards = Resources.LoadAll<Card>("Cards");

        foreach (Card card in AllCards)
        {
            card.OnLoad();
        }
    }

    public static Coroutine StartStaticCoroutine(IEnumerator method)
    {
        return Instance.StartCoroutine(method);
    }

    public static void StopStaticCoroutine(Coroutine coroutine)
    {
        Instance.StopCoroutine(coroutine);
    }
}