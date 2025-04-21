using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCardState : IState
{
    bool temp = false; //ANV�NDS F�R ATT K�NNA N�R WAITTIME �R DONE

    int tempManaCostSave; //ANV�NDS F�R ATT TEMPOR�RT SPARA MANA COSTNADEN FOR NOW

    GameBehaviour gameBehaviour;
    public virtual IState Enter(GameBehaviour gameBehaviour)
    {
        Debug.Log("PLAY CARD!!");
        this.gameBehaviour = gameBehaviour;
        gameBehaviour.UpdateStatusText("Card playing");

        Debug.Log("Play card effect");
        //MANA SHOULD BE REMOVED FROM WITHIN THE CARD INSTEAD
        tempManaCostSave = gameBehaviour.cardHand.cardBeingPlayed.GetCost();
        gameBehaviour.LoseMana(tempManaCostSave);

        gameBehaviour.StartCoroutine(Wait()); //s�tter temp till true

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
        if (temp && !gameBehaviour.cardHand.cardIsPlaying)
        {
            Debug.Log("Damage enemy");
            //gameBehaviour.EnemyTakeDamage(5);  //DET H�R H�NDER VARJE FRAME HELA TIDEN

            //when played effect is done // could possibly be a cooldown timer have timer in gameBehaviour and return? would that work?
            //check enemy hp,
            //if enemy hp <= 0 return gameWon
            //else return selectionState


            if (gameBehaviour.encounterManager.currentEncounterEnemy.HP > 0)
            {
                return new SelectionState();
            }
            else
            {
                gameBehaviour.encounterManager.currentEncounterEnemy.DeathEvent();
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
