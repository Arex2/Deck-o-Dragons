using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
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
    public bool CancelButtonPressed => cancelButtonPressed;
    private bool cancelButtonPressed;

    //player stats, should maybe be moved? or script renamed
    //OBS MaxHP and HP is instead used from Target superclass
    private int mana;
    private int maxMana = 8;

    public int Mana => mana;

    //ha koppling till CardHand och Controls
    [SerializeField]
    public ControlsV2 controls;
    [SerializeField]
    public CardHand cardHand;

    //End turn button
    [Space]
    [SerializeField]
    Button button;
    CanvasGroup buttonCanvasGroup;
    [SerializeField]
    TMP_Text buttonText;
    string buttonDefaultText;

    //temp canvas text
    [Space]
    [SerializeField]
    private TMP_Text statusText;
    [SerializeField]
    private TMP_Text manaText;
    [SerializeField]
    private TMP_Text playerHealthText;
    //[SerializeField]
    //private TMP_Text enemyHealthText;

    [SerializeField]
    private Slider hpSlider;

    [SerializeField]
    public IndicatorManager indicatorManager;

    [SerializeField]
    private ScreenShake screenShake;

    //Reference to enemy script
    [SerializeField]
    public EncounterManager encounterManager;

    protected override void Awake()
    {
        Instance = this;

        buttonCanvasGroup = button.GetComponentInChildren<CanvasGroup>(true);

        buttonCanvasGroup.alpha = 0;
        buttonCanvasGroup.blocksRaycasts = false;

        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        buttonDefaultText = buttonText.text;

        mana = maxMana;

        manaText.text = mana.ToString();
        //enemyHealthText.text = enemyHp.ToString();
        playerHealthText.text = HP.ToString();

        hpSlider.maxValue = MaxHP;
        hpSlider.value = HP;

        encounterManager.InctanceNextEncounter();
    }

    //update status text
    public void UpdateStatusText(string text)
    {
        statusText.text = text;
    }

    //end turn button
    public void ButtonPress()
    {
        if (!cardHand.SelectingCards)
        {
            encounterManager.currentEncounterEnemy.StartTurn();
        }

        buttonPressed = true;
    }

    public void SetButtonText(string text)
    {
        if (buttonText.text == text)
        {
            return;
        }

        buttonText.text = text;
    }

    public void ResetButtonText()
    {
        SetButtonText(buttonDefaultText);
    }

    public void EnableButton()
    {
        buttonPressed = false;

        buttonCanvasGroup.blocksRaycasts = true;

        buttonCanvasGroup.DOKill();
        buttonCanvasGroup.DOFade(1, 0.25f);
    }

    public void DisableButton()
    {
        buttonPressed = false;

        buttonCanvasGroup.blocksRaycasts = false;

        buttonCanvasGroup.DOKill();
        buttonCanvasGroup.DOFade(0, 0.25f);
    }

    protected override void UpdateHP()
    {
        if (hpSlider.value > HP)
        {
            screenShake.StartShake();
        }
        hpSlider.value = HP; 
        //StartCoroutine(UpdateHealthBar());
        playerHealthText.text = HP.ToString();
        indicatorManager.ClearIndicators();
    }

    //Verkade som att det redan var någon incrimental effekt på slidern/hpBaren
    private IEnumerator UpdateHealthBar()
    {
        float duration = 1f;
        float elapsedTime = 0f;
        float startHealth = hpSlider.value;

        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            hpSlider.value = Mathf.Lerp(startHealth, HP, elapsedTime);
        }
        yield return null;
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

    public void NewEncounter()
    {
        HP = MaxHP;
        mana = maxMana;
        cardHand.EmptyHand();
        encounterManager.InctanceNextEncounter();
    }
}
