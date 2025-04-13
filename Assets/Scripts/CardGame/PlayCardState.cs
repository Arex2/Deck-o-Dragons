using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCardState : IState
{
    bool temp = false; //ANVÄNDS FÖR ATT KÄNNA NÄR WAITTIME ÄR DONE

    int tempManaCostSave; //ANVÄNDS FÖR ATT TEMPORÄRT SPARA MANA COSTNADEN FOR NOW

    GameBehaviour gameBehaviour;
    public virtual IState Enter(GameBehaviour gameBehaviour)
    {
        Debug.Log("PLAY CARD!!");
        this.gameBehaviour = gameBehaviour;
        gameBehaviour.UpdateStatusText("Card playing");

        Debug.Log("Play card effect");
        //MANA SHOULD BE REMOVED FROM WITHIN THE CARD INSTEAD
        tempManaCostSave = gameBehaviour.cardHand.cardBeingPlayed.GetComponent<CardObject>().GetCost();
        gameBehaviour.LoseMana(tempManaCostSave);


        gameBehaviour.StartCoroutine(Wait()); //sätter temp till true

        return null;
    }
    IEnumerator Wait()
    {
        //Debug.Log("Wait start " + Time.time);
        yield return new WaitForSeconds(1.5f);
        //Debug.Log("Wait over " + Time.time);
        temp = true;
    }

    public virtual IEnumerator PlayEffects(GameBehaviour gameBehaviour)
    {
        return null;
    }

    public virtual IState Execute()
    {
        if (temp)
        {
            Debug.Log("Damage enemy");
            //gameBehaviour.EnemyTakeDamage(5);  //DET HÄR HÄNDER VARJE FRAME HELA TIDEN

            //when played effect is done // could possibly be a cooldown timer have timer in gameBehaviour and return? would that work?
            //check enemy hp,
            //if enemy hp <= 0 return gameWon
            //else return selectionState


            if (gameBehaviour.enemyBoss.HP > 0)
            {
                return new SelectionState();
            }
            else
            {
                gameBehaviour.enemyBoss.DeathEvent();
                Debug.Log("Enemy death");
                return new BattleOverState();
            }
        }
        else return null;
        
    }

    public virtual IState Exit()
    {
        return null;
    }




}
