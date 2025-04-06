using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCardState : IState
{
    GameBehaviour gameBehaviour;
    public virtual IState Enter(GameBehaviour gameBehaviour)
    {
        Debug.Log("PLAY CARD!!");
        this.gameBehaviour = gameBehaviour;
        gameBehaviour.UpdateStatusText("Card playing");

        gameBehaviour.EnemyTakeDamage(5);//temp flyttad för att den körde så ofta i execute
        return null;
    }


    public virtual IState Execute()
    {
        //if endturn button input
        Debug.Log("Play card effect");
        Debug.Log("Damage enemy");
        //gameBehaviour.EnemyTakeDamage(5);  //DET HÄR HÄNDER VARJE FRAME HELA TIDEN
        
        //when played effect is done // could possibly be a cooldown timer have timer in gameBehaviour and return? would that work?
                 //check enemy hp,
                 //if enemy hp <= 0 return gameWon
                 //else return selectionState

        if (gameBehaviour.EnemyHp > 0)
        {
            return new SelectionState();
        }
        else
        {
            Debug.Log("Enemy death");
            return new BattleOverState();
        }
    }

    public virtual IState Exit()
    {
        return null;
    }

}
