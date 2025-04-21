using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    private bool endTurn;

    public bool EndTurn
    { get { return endTurn; } }

    private bool cardsSelected;
    public bool CardSelected
    { get { return cardsSelected; }
        set { cardsSelected = value; }
    }

    //player stats, should maybe be moved? or script renamed
    //OBS MaxHP and HP is instead used from Target superclass
    private int mana;
    private int maxMana = 8;
    
    public int Mana 
    { get { return mana; } }

    //ha koppling till CardHand och Controls
    [SerializeField]
    public ControlsV2 controls;
    [SerializeField]
    public CardHand cardHand;

    //End turn button
    [SerializeField]
    Button endTurnButton;
    [SerializeField]
    TMP_Text endTurnButtonText;
    string endTurnButtonStartText;

    //temp canvas text
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

    //Reference to enemy script
    [SerializeField]
    public EnemyBoss enemyBoss;

    protected override void Awake()
    {
        Instance = this;

        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        endTurnButtonStartText = endTurnButtonText.text;

        mana = maxMana;

        manaText.text = mana.ToString();
        //enemyHealthText.text = enemyHp.ToString();
        playerHealthText.text = HP.ToString();

        hpSlider.maxValue = MaxHP;
        hpSlider.value = HP;
    }

    //update status text
    public void UpdateStatusText(string text)
    {
        statusText.text = text;
    }

    //end turn button
    public void ButtonPress()
    {
        if(cardHand.choosingCardsToAffect)
        {
            endTurnButton.interactable = false;
            cardsSelected = true;
        }
        else EndCurrentTurn();
    }

    public void EndCurrentTurn()
    {
        if (!cardHand.choosingCardsToKeepAfterDiscard)
        {
            enemyBoss.StartTurn();
        }

        endTurn = true;
        //stäng av knapp
        endTurnButton.interactable = false;
    }

    public void SetEndTurnButtonText(string text)
    {
        endTurnButtonText.text = text;
    }

    public void ResetEndTurnButtonText()
    {
        SetEndTurnButtonText(endTurnButtonStartText);
    }

    public void NewTurn()
    {
        endTurn = false;
        //sätt på knapp
        endTurnButton.interactable = true;
    }

    protected override void UpdateHP()
    {
        hpSlider.value = HP;
        playerHealthText.text = HP.ToString();
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
}
