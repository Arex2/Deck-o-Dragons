using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    private TMP_Text playerHealthText;
    [SerializeField]
    private TMP_Text enemyHealthText;

    // Start is called before the first frame update
    void Start()
    {
        hp = maxHp;

        enemyHealthText.text = enemyHp.ToString();
        playerHealthText.text = hp.ToString();
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
        playerHealthText.text = hp.ToString();
    }

    public void LoseMana(int count)
    {
        mana -= count;
    }

    public void ResetMana()
    {
        mana = maxMana;
    }
}
