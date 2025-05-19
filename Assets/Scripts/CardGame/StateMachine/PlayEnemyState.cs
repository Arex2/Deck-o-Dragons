using System.Collections;
using UnityEngine;

public class PlayEnemyState : IState
{
    bool coroutineOver = false; //ANVÄNDS FÖR ATT KÄNNA NÄR WAITTIME ÄR DONE

    GameBehaviour gameBehaviour;
    public void Enter(GameBehaviour gameBehaviour)
    {
        Debug.Log("ENEMY TURN!!");
        this.gameBehaviour = gameBehaviour;
        //gameBehaviour.UpdateStatusText("Enemy Turn");
        Target.TurnStart.Invoke(Team.Enemy);
    }

    public IEnumerator Coroutine()
    {
        //Debug.Log("Wait start " + Time.time);
        yield return new WaitForSeconds(1);
        //Debug.Log("Wait over " + Time.time);
        coroutineOver = true;
    }

    public IState Execute()
    {
        if (coroutineOver && CardVFXManager.ActiveVFXCount <= 0)
        {

            //CARDS PLAY THEMSELVES

            //play own cards
            //Debug.Log("Damage the player");
            //gameBehaviour.Hurt(3);
            //Debug.Log("Heal itself (?)");

            //check player hp
            //if player hp <= 0, return gameLost

            if (BattleOverState.BattleOverCheck(out Team winningTeam))
            {
                return new BattleOverState(winningTeam);
            }
            else
            {
                Target.TurnEnd.Invoke(Team.Enemy);
                return new NewTurnState();
            }
        }
        else return null;
    }

    public void Exit()
    {

    }
}
