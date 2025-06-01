using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameBehaviour : Target
{
    public static GameBehaviour Instance { get; private set; }

    #region stuff for Target class
    public override Team Team => Team.Player;
    public override Bounds GetWorldBounds()
    {
        return new Bounds(transform.position, transform.localScale);
    }
    #endregion

    public bool ButtonPressed => buttonPressed;
    private bool buttonPressed;

    //player stats
    //OBS MaxHP and HP is used from Target superclass
    private int mana;
    private int maxMana = 8;

    public int Mana => mana;

    //ha koppling till CardHand och Controls
    [SerializeField]
    public ControlsV2 controls;
    [SerializeField]
    public CardHand cardHand;

    public CardgameStatusButton StatusButton => statusButton;

    //End turn button
    [Space]
    [SerializeField]
    CardgameStatusButton statusButton;
    CanvasGroup statusButtonCanvasGroup;

    //temp canvas text
    /*
    [Space]
    [SerializeField]
    private TMP_Text statusText;
    */
    [SerializeField]
    private TMP_Text manaText;
    [SerializeField]
    private TMP_Text playerHealthText;
    private string _playerHealthFormat;
    //[SerializeField]
    //private TMP_Text enemyHealthText;

    //Damage text - Harriet
    [SerializeField]
    private GameObject FloatingTextPrefab;

    [SerializeField]
    private Slider hpSlider;
    //Health slider animation variables - Harriet
    bool updateHealthSlider;
    float elapsedTime = 0f;
    float desiredDuration = 0.3f;
    float healthSliderStartPos;

    [SerializeField] private AudioClip[] damageTakenSound;

    [SerializeField]
    public IndicatorManager indicatorManager;

    [SerializeField]
    private ScreenShake screenShake;

    [Space]
    [SerializeField]
    private
#if UNITY_EDITOR
        new
#endif
        Camera camera;

    [SerializeField] private RectTransform hitPositionRect;

    [Header("Battle Over")]
    [SerializeField] private CanvasGroup battleOverCanvas;
    [SerializeField] private TMP_Text battleOverResultText;

    [Header("Status Effects Display")]
    [SerializeField] private RectTransform bottomHud;
    [SerializeField] private RectTransform cardPositions;
    private float _bottomHudStartingPosY;
    [SerializeField] private float statusEffectDisplaySize = 40;
    private bool _showingStatusEffectDisplay = false;

    //Reference to enemy script
    /*
    [SerializeField]
    public EncounterManager encounterManager;
    */

    protected override void Awake()
    {
        Instance = this;

        _bottomHudStartingPosY = bottomHud.anchoredPosition.y;

        _playerHealthFormat = playerHealthText.text;

        statusButtonCanvasGroup = statusButton.GetComponentInChildren<CanvasGroup>(true);

        statusButtonCanvasGroup.blocksRaycasts = false;

        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        mana = maxMana;

        manaText.text = mana.ToString();

        hpSlider.maxValue = MaxHP;

        hpSlider.value = HP;  //prevents health animation at start

        UpdateHP();

        //encounterManager.InctanceNextEncounter();
    }


    private void Update()
    {
        if (updateHealthSlider)
        {
            elapsedTime += Time.deltaTime;
            float percentage = elapsedTime / desiredDuration;
            hpSlider.value = Mathf.Lerp(healthSliderStartPos, HP, percentage);

            float indicatorOpacity = Mathf.Lerp(1, 0, percentage);
            //update health indicator
            indicatorManager.UpdateIndicatorsForOldCard();
            indicatorManager.ChangeIndicatorOpacity(indicatorOpacity);
        }

        if (updateHealthSlider && hpSlider.value == HP)
        {
            elapsedTime = 0;
            updateHealthSlider = false;
            //update health indicator
            //indicatorManager.UpdateIndicatorsForOldCard();
        }
    }

    //update status text
    /*
    public void UpdateStatusText(string text)
    {
        statusText.text = text;
    }
    */

    //end turn button
    public void ButtonPress()
    {
        /*
        if (!cardHand.SelectingCards)
        {
            encounterManager.currentEncounterEnemy.StartTurn();
        }
        */

        buttonPressed = true;
    }

    public void SetButtonText(string text)
    {
        statusButton.SetOverrideCurrentText(text);
    }

    public void ResetButtonText()
    {
        statusButton.SetOverrideCurrentText(null);
    }

    public void EnableButton()
    {
        buttonPressed = false;

        statusButtonCanvasGroup.blocksRaycasts = true;
    }

    public void DisableButton()
    {
        buttonPressed = false;

        statusButtonCanvasGroup.blocksRaycasts = false;
    }

    public void BattleOver(string result)
    {
        battleOverCanvas.DOFade(1, 1);

        battleOverResultText.text = result;
    }

    public override void Hurt(AttackData attackData)
    {
        base.Hurt(attackData);

        ShowFloatingText(attackData);

        if (attackData > 0)
        {
            screenShake.StartShake();
            if (AudioManager.Instance != null)
            {
                int randomIndex = Random.Range(0, damageTakenSound.Length);
                AudioManager.Instance.PlaySFX(damageTakenSound[randomIndex]);
            }
        }
    }


    protected override void UpdateHP()
    {
        base.UpdateHP();
        /*
        if (hpSlider.value > HP)
        {
            screenShake.StartShake();
        }
        */

        //hpSlider.value = HP; 
        healthSliderStartPos = hpSlider.value;
        updateHealthSlider = true; //controls health slider anim from update

        playerHealthText.text = string.Format(_playerHealthFormat, Mathf.Ceil(HP), MaxHP);
        //indicatorManager.ClearIndicators();
    }
    //Damage text
    private void ShowFloatingText(AttackData attackData)
    {
        //GetComponent<GameObject>().GetChi
        Vector3 spawnPos = hpSlider.fillRect.transform.GetChild(1).transform.position;
        GameObject obj = Instantiate(FloatingTextPrefab, new Vector3(camera.ScreenToWorldPoint(spawnPos).x, camera.ScreenToWorldPoint(spawnPos).y, 0), Quaternion.identity);
        obj.GetComponent<TextMeshPro>().text = attackData.ToString();
        //Debug.Log("Spawn pos:  " + new Vector3(camera.ScreenToWorldPoint(spawnPos).x, camera.ScreenToWorldPoint(spawnPos).y, 0));
        //obj.GetComponent<TextMeshPro>().color = Random.ColorHSV();
        obj.GetComponent<TextMeshPro>().color = Color.red;
    }

    protected override void UpdateStatusEffects()
    {
        base.UpdateStatusEffects();

        bool hasStatusEffects = StatusEffects.Count > 0;

        if (_showingStatusEffectDisplay == hasStatusEffects)
        {
            return;
        }

        _showingStatusEffectDisplay = hasStatusEffects;

        bottomHud.DOKill();

        if (hasStatusEffects)
        {
            bottomHud.DOAnchorPosY(_bottomHudStartingPosY + statusEffectDisplaySize, 0.5f);
            cardPositions.sizeDelta = new Vector2(0, -statusEffectDisplaySize);
            cardPositions.anchoredPosition = new Vector2(0, statusEffectDisplaySize / 2);
        }
        else
        {
            bottomHud.DOAnchorPosY(_bottomHudStartingPosY, 0.5f);
            cardPositions.sizeDelta = Vector2.zero;
            cardPositions.anchoredPosition = Vector2.zero;
        }

        cardHand.UpdateScreenPositions();
        cardHand.UpdateCardPositions();
    }

    public void GainMana(int count)
    {
        mana += count;
        UpdateMana();
    }

    public void LoseMana(int count)
    {
        mana -= count;
        UpdateMana();
    }

    public void ResetMana()
    {
        mana = maxMana;
        UpdateMana();
    }

    public void UpdateMana()
    {
        manaText.text = mana.ToString();
    }

    public void CheckCardAvailability()
    {
        cardHand.CheckCardAvailability();
    }

    public void UpdateCardIndicators()
    {
        cardHand.UpdateCardIndicator();
    }

    public void SwitchToEggScene()
    {
        SceneManager.LoadScene(1);
        //viktig
        DragonActive.doCheck = true;
    }

    /// <summary>
    /// Check if player has enough mana
    /// to play selected card.
    /// </summary>
    /// <param name="cardCost">mana cost of selected card</param>
    /// <returns>true for enough mana, false for not enough mana</returns>
    public bool CheckMana(int cardCost)
    {
        if (cardCost <= mana)
            return true;
        else return false;
    }

    public override Vector2 GetHitPosition() => camera.ScreenToWorldPoint(hitPositionRect.position);

    /*
    public void NewEncounter()
    {
        HP = MaxHP;
        mana = maxMana;
        cardHand.EmptyHand();
        encounterManager.InctanceNextEncounter();
    }
    */
}
