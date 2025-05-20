using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class EnemyBoss : Target
{
    /*
     * Mana and health variables
     */

    public Sprite Sprite => spriteRenderer.sprite;
    public string Name { get; private set; } = "Boss";

    [Space]
    [SerializeField] private UnityEngine.UI.Slider healthSlider;
    //Health slider animation variables - Harriet
    bool updateHealthSlider;
    float elapsedTime = 0f;
    float desiredDuration = 0.3f;
    float healthSliderStartPos;

    //indicatorManager for health slider - Harriet
    [SerializeField] IndicatorManager indicatorManager;

    [Space]
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private int maxMana;
    private int currentMana;
    private string healthTextFormat;

    [Space]
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
    [SerializeField] private AudioClip[] damageTakenSound;

    [SerializeField] private List<Sprite> enemySprites;
    [SerializeField] private List<string> enemyNames;
    [SerializeField] private TMP_Text nameText;
    private SpriteRenderer spriteRenderer;

    [Space]
    [SerializeField] private Animator bushes;

    //Damage text - Harriet
    [SerializeField]
    private GameObject FloatingTextPrefab;

    public override Team Team => Team.Enemy;

    protected override void Awake()
    {
        base.Awake();
        if (EnemyScalingManager.Instance != null)
        {
            maxMana = EnemyScalingManager.Instance.GetScaledMana();
            MaxHP = EnemyScalingManager.Instance.GetScaledHealth();

        }
        healthTextFormat = healthText.text;
        healthSlider.maxValue = MaxHP;

        HP = MaxHP;
        healthSlider.value = HP;
        UpdateHP();

        originalPosition = transform.localPosition;
        transform.localPosition = new Vector3(0, -6);

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (ProgressManager.Instance != null)
        {
            int level = ProgressManager.Instance.GetCurrentLevel();

            if (enemySprites.Count - 1 > level)
            {
                spriteRenderer.sprite = enemySprites[level];

                Name = enemyNames[level];
                nameText.text = "???";
            }
        }
    }

    public void Appear()
    {
        transform.DOLocalMove(originalPosition, 1).SetEase(Ease.OutExpo).onComplete = () =>
        {
            spriteRenderer.sortingLayerName = "Default";
        };

        if (bushes != null)
        {
            bushes.enabled = true;
        }

        nameText.text = Name;
    }

    private void Update()
    {
        if (updateHealthSlider)
        {
            elapsedTime += Time.deltaTime;
            float percentage = elapsedTime / desiredDuration;
            healthSlider.value = Mathf.Lerp(healthSliderStartPos, HP, percentage);
            float indicatorOpacity = Mathf.Lerp(1, 0, percentage);
            //update health indicator
            indicatorManager.UpdateIndicatorsForOldCard();
            indicatorManager.ChangeIndicatorOpacity(indicatorOpacity);
        }

        if (updateHealthSlider && healthSlider.value == HP)
        {
            elapsedTime = 0;
            updateHealthSlider = false;
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
        PlayCards();
    }

    private void PlayCards()
    {
        while (true)
        {
            if (currentMana <= 0 || !CanPlayCard())
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

        for (int i = 0; i < cardsAvailable.Count; i++)
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

        if (cardToCheck.Category == CardCategory.Defense)
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
        //healthSlider.value = HP;
        healthSliderStartPos = healthSlider.value;
        updateHealthSlider = true;
        healthText.text = string.Format(healthTextFormat, Mathf.Ceil(HP), MaxHP);
    }

    public override void Hurt(AttackData attackData)
    {
        base.Hurt(attackData);

        StartCoroutine(ShakeCoroutine());
        if (AudioManager.Instance != null)
        {
            int randomIndex = Random.Range(0, damageTakenSound.Length);
            AudioManager.Instance.PlaySFX(damageTakenSound[randomIndex]);
        }

        if(FloatingTextPrefab)
        {
            ShowFloatingText(attackData);
        }
    }

    public override void OnDeath()
    {
        base.OnDeath();
        if (EnemyScalingManager.Instance != null && ProgressManager.Instance != null)
        {
            ProgressManager.Instance.IncreaseLevel();
            EnemyScalingManager.Instance.AdvanceScaling();
        }
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

    //Damage text
    private void ShowFloatingText(AttackData attackData)
    {
        GameObject obj = Instantiate(FloatingTextPrefab, transform.position, Quaternion.identity);
        obj.GetComponent<TextMeshPro>().text = attackData.ToString();
        //obj.GetComponent<TextMeshPro>().color = Random.ColorHSV();
        obj.GetComponent<TextMeshPro>().color = Color.red;
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
