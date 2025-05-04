using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBoss : Target
{
    /*
     * Mana and health variables
     */

    //notera måste lägga tillbaka scriptet på enemyn för att det ska fungera
    [SerializeField] private Slider healthSlider; 

    [SerializeField] private TMP_Text healthText;
    [SerializeField] private int maxMana;
    private int currentMana;
    private string healthTextFormat;

    [SerializeField] List<Card> cardsAvailable = new List<Card>();

    /*
     * Shake object when taking damage
     */
    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeMagnitude;
    private Vector3 originalPosition;

    /*
     * Audio
     */
    [SerializeField] private AudioClip damageTakenSound;

    [SerializeField] private List<Sprite> enemySprites;
    private SpriteRenderer spriteRenderer;

    public override Team Team => Team.Enemy;

    protected override void Awake()
    {
        base.Awake();

        if (EnemyScalingManager.Instance != null)
        {
            maxMana = EnemyScalingManager.Instance.GetScaledMana();
        }
        healthTextFormat = healthText.text;
        healthSlider.maxValue = MaxHP;
        UpdateHP();
        originalPosition = transform.localPosition;
         spriteRenderer = GetComponent<SpriteRenderer>();

        if (ProgressManager.Instance != null)
        {
           
            if (enemySprites.Count - 1 > ProgressManager.Instance.GetCurrentLevel())
            {
                spriteRenderer.sprite = enemySprites[ProgressManager.Instance.GetCurrentLevel()];
            }
        }
    }

    public override void OnTurnStart()
    {
        base.OnTurnStart();

        if (!Dead)
        {
            StartTurn();
        }
    }

    public void StartTurn()
    {
        currentMana = maxMana;
        print(currentMana);
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

            if (CheckIfPlayable(currentCard))
            {
                currentMana -= currentCard.Cost;

                currentCard.VFXSpawnOrigin = transform.position;
                StartCoroutine(currentCard.Play(this));
            }
        }

        EndTurn();
    }

    private Card PickRandomCard()
    {
        int index = Random.Range(0, cardsAvailable.Count);  
        
        return cardsAvailable[index];
    }

    //Check if the enemy can afford to play a card based on current mana
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

    private bool CheckIfPlayable(Card cardToCheck)
    {
        bool playCard = true;

        if(cardToCheck.Category == CardCategory.Defense)
        {
            if (HP == MaxHP)
            {
                playCard = false;
            }
        }
        return playCard;
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

    public override Vector2 GetHitPosition() => transform.position;

    protected override void UpdateHP()
    {
        base.UpdateHP();
         healthText.text = string.Format(healthTextFormat, HP.ToString());
        healthSlider.value = HP;
    }

    public override void Hurt(AttackData attackData)
    {
        base.Hurt(attackData);

        StartCoroutine(ShakeCoroutine());
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(damageTakenSound);
        }
    }

    public override void OnDeath()
    {
        base.OnDeath();
        ProgressManager.Instance.IncreaseLevel();
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

    /*
    //Behövs inte med CardVFX trigger systemet - Ruben
    //För att trigga shake när dmg projektiler kommer tillräckligt nära
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Projectile"))
        {
            StartCoroutine(ShakeCoroutine());
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(damageTakenSound);
            }
            collision.GetComponent<AttackEffect>().DestroySelf();
        }
    }
    */
}
