using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameBehaviour : Target
{
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

    //player stats, should maybe be moved? or script renamed
    //OBS MaxHP and HP is instead used from Target superclass
    private int mana;
    private int maxMana = 8;
    private int hpNew;
    private int maxHpNew = 10;

    public int HpNew
    { get { return hpNew; } }
    

    public int Mana 
    { get { return mana; } }


    //temp enemy stats
    private int enemyHp = 20;
    public int EnemyHp
    { get { return enemyHp; } }


    //ha koppling till CardHand och Controls
    [SerializeField]
    public Controls controls;
    [SerializeField]
    public CardHand cardHand;



    //temp canvas text
    [SerializeField]
    private TMP_Text statusText;
    [SerializeField]
    private TMP_Text manaText;
    [SerializeField]
    private TMP_Text playerHealthText;
    [SerializeField]
    private TMP_Text enemyHealthText;

    [SerializeField]
    private Slider hpSlider;

    // Start is called before the first frame update
    void Start()
    {
        hpNew = maxHpNew;
        mana = maxMana;

        manaText.text = mana.ToString();
        enemyHealthText.text = enemyHp.ToString();
        playerHealthText.text = HP.ToString();

        hpSlider.maxValue = MaxHP;
        hpSlider.value = HP;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //temp enemy take damage method
    public void EnemyTakeDamage(int count)
    {
        Debug.Log("enemy dmg take: " + count);
        enemyHp -= count;
        enemyHealthText.text = enemyHp.ToString();
    }

    //update status text
    public void UpdateStatusText(string text)
    {
        statusText.text = text;
    }

    //end turn button
    public void EndCurrentTurn()
    {
        endTurn = true;
    }

    public void NewTurn()
    {
        endTurn = false;
    }

    public void LoseHp(int count)
    {
        hpNew -= count;
        hpSlider.value = hpNew;
        playerHealthText.text = hpNew.ToString();
    }

    public override void Hurt(float amount)
    {
        base.Hurt(amount);
        hpSlider.value = HP;
        playerHealthText.text = HP.ToString();
    }

    public void LoseMana(int count)
    {
        mana -= count;
        manaText.text = mana.ToString();
    }

    public void ResetMana()
    {
        mana = maxMana;
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
