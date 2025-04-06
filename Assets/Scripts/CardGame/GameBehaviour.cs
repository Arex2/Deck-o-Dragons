using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameBehaviour : MonoBehaviour
{
    private bool endTurn;

    public bool EndTurn
    { get { return endTurn; } }

    //player stats, should maybe be moved? or script renamed
    private int mana;
    private int maxMana = 8;
    private int hp;
    private int maxHp = 10;

    public int Hp
    { get { return hp; } }

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
        hp = maxHp;
        mana = maxMana;

        manaText.text = mana.ToString();
        enemyHealthText.text = enemyHp.ToString();
        playerHealthText.text = hp.ToString();

        hpSlider.maxValue = maxHp;
        hpSlider.value = hp;
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
        hp -= count;
        hpSlider.value = hp;
        playerHealthText.text = hp.ToString();
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
