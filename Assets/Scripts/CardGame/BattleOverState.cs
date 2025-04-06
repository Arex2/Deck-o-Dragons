using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleOverState : IState
{
    GameBehaviour gameBehaviour;
    public virtual IState Enter(GameBehaviour gameBehaviour)
    {
        Debug.Log("BATTLE OVER");
        this.gameBehaviour = gameBehaviour;
        //gameBehaviour.NewTurn();
        gameBehaviour.UpdateStatusText("BATTLE OVER");
        return null;
    }


    public virtual IState Execute()
    {
        return null;
    }
    public virtual IState Exit()
    {
        return null;
    }
}
