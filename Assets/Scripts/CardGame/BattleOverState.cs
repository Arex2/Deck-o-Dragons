using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;


public class BattleOverState : IState
{
    bool temp = false; //ANV�NDS F�R ATT K�NNA N�R WAITTIME �R DONE

    GameBehaviour gameBehaviour;
    public virtual IState Enter(GameBehaviour gameBehaviour)
    {
        Debug.Log("BATTLE OVER");
        this.gameBehaviour = gameBehaviour;
        //gameBehaviour.NewTurn();
        gameBehaviour.UpdateStatusText("BATTLE OVER");

        gameBehaviour.StartCoroutine(Wait()); //s�tter temp till true

        return null;
    }

    IEnumerator Wait()
    {
        //Debug.Log("Wait start " + Time.time);
        yield return new WaitForSeconds(2);
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
            Debug.Log("New Encounter");
            gameBehaviour.NewEncounter();
            return new SetupState();
        }
        return null;
    }
    public virtual IState Exit()
    {
        return null;
    }
}
