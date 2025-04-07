using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class EnemyBoss : Target
{
    [SerializeField] private int maxMana;
    [SerializeField] List<Card> cardsAvailable = new List<Card>();
    [SerializeField] private TMP_Text healthText;

    //Shake object when taking damage
    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeMagnitude;
    private Vector3 originalPosition;


    private int currentMana;
    private string healthTextFormat;
    private AudioSource audioSource;
    [SerializeField] private AudioClip damageTakenSound;

    public override Team Team => Team.Enemy;

    protected override void Awake()
    {
        base.Awake();
        healthTextFormat = healthText.text;
        UpdateHP();
        originalPosition = transform.position;
        audioSource = GetComponent<AudioSource>();
    }

    public void StartTurn()
    {
        currentMana = maxMana;
        PlayCards();
    }

    private void PlayCards()
    {
        while (true)
        {
            if(currentMana <= 0 || !CanPlayCard())
            {
                break;
            }


            Card currentCard = PickRandomCard();
            currentCard.Play(this);
            currentMana -= currentCard.Cost;
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

    private bool CanPlayCard()
    {
        bool canPlay = false;

        for(int i = 0; i < cardsAvailable.Count; i++)
        {
            if (cardsAvailable[i].Cost <= currentMana)
            {
                canPlay = true;
            }
        }
        return canPlay;
    }
    private void EndTurn()
    {
        /*
         * Code to end turn here
         */
    }

    public override Bounds GetWorldBounds()
    {
        return new Bounds(transform.position, transform.localScale);
    }
    protected override void UpdateHP()
    {
        base.UpdateHP();
         healthText.text = string.Format(healthTextFormat, hp.ToString());
    }

    public override void Hurt(float amount)
    {
        base.Hurt(amount);
        StartCoroutine(ShakeCoroutine());
        audioSource.clip = damageTakenSound;
        audioSource.Play();

    }

    private IEnumerator ShakeCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }

    public void DeathEvent()
    {
        StartCoroutine(RotateOverTime(Quaternion.Euler(0, 0, 90), 0.3f));
    }

    private IEnumerator RotateOverTime(Quaternion rotationOffset, float time)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * rotationOffset;
        float elapsed = 0f;

        while (elapsed < time)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsed / time);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRotation;
    }
}
