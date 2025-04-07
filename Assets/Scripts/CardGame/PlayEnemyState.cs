using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayEnemyState : IState
{
    bool temp = false; //ANVÄNDS FÖR ATT KÄNNA NÄR WAITTIME ÄR DONE

    GameBehaviour gameBehaviour;
    public virtual IState Enter(GameBehaviour gameBehaviour)
    {
        Debug.Log("ENEMY TURN!!");
        this.gameBehaviour = gameBehaviour;
        gameBehaviour.UpdateStatusText("Enemy Turn");


        //play card effect


        gameBehaviour.StartCoroutine(Wait()); //sätter temp till true

        return null;
    }

    IEnumerator Wait()
    {
        //Debug.Log("Wait start " + Time.time);
        yield return new WaitForSeconds(1);
        //Debug.Log("Wait over " + Time.time);
        temp = true;
    }

    public virtual IState Execute()
    {
        if (temp)
        {

            //CARDS PLAY THEMSELVES

            //play own cards
            //Debug.Log("Damage the player");
            //gameBehaviour.Hurt(3);
            //Debug.Log("Heal itself (?)");

            //check player hp
            //if player hp <= 0, return gameLost
            //else return setupState
            if (gameBehaviour.HP <= 0)
            {
                Debug.Log("RETURN GAME LOST " + gameBehaviour.HP);
                return new BattleOverState();
            }
            else return new SetupState();
        }
        else return null;
    }

    public virtual IState Exit()
    {
        return null;
    }
}
