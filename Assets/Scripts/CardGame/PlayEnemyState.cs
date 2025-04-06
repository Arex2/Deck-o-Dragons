using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayEnemyState : IState
{
    GameBehaviour gameBehaviour;
    public virtual IState Enter(GameBehaviour gameBehaviour)
    {
        Debug.Log("ENEMY TURN!!");
        this.gameBehaviour = gameBehaviour;
        gameBehaviour.UpdateStatusText("Enemy Turn");
        //play own cards
        return null;
    }


    public virtual IState Execute()
    {
        Debug.Log("Damage the player");
        //gameBehaviour.LoseHp(3);
        //Debug.Log("Heal itself (?)");


        //check player hp
        //if player hp <= 0, return gameLost
        //else return setupState
        if (gameBehaviour.Hp <= 0)
        {
            Debug.Log("RETURN GAME LOST");
            return new BattleOverState();
        }
        else return new SetupState();

    }

    public virtual IState Exit()
    {
        return null;
    }
}
