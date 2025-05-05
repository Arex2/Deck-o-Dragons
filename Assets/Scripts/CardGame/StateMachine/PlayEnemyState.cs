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
        Target.TurnStart.Invoke(Team.Enemy);

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

    public virtual IEnumerator PlayEffects(GameBehaviour gameBehaviour)
    {
        return null;
    }

    public virtual IState Execute()
    {
        if (temp && CardVFXManager.ActiveVFXCount <= 0)
        {

            //CARDS PLAY THEMSELVES

            //play own cards
            //Debug.Log("Damage the player");
            //gameBehaviour.Hurt(3);
            //Debug.Log("Heal itself (?)");

            //check player hp
            //if player hp <= 0, return gameLost

            BattleOverState battleOverState = BattleOverState.BattleOverCheck();

            if (battleOverState != null)
            {
                return battleOverState;
            }
            else
            {
                Target.TurnEnd.Invoke(Team.Enemy);
                return new SetupState();
            }
        }
        else return null;
    }

    public virtual IState Exit()
    {
        return null;
    }
}
