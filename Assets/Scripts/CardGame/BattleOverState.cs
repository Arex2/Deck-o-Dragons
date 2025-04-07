using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleOverState : IState
{
    bool temp = false; //ANVÄNDS FÖR ATT KÄNNA NÄR WAITTIME ÄR DONE

    GameBehaviour gameBehaviour;
    public virtual IState Enter(GameBehaviour gameBehaviour)
    {
        Debug.Log("BATTLE OVER");
        this.gameBehaviour = gameBehaviour;
        //gameBehaviour.NewTurn();
        gameBehaviour.UpdateStatusText("BATTLE OVER");

        gameBehaviour.StartCoroutine(Wait()); //sätter temp till true

        return null;
    }

    IEnumerator Wait()
    {
        //Debug.Log("Wait start " + Time.time);
        yield return new WaitForSeconds(2);
        //Debug.Log("Wait over " + Time.time);
        temp = true;
    }

    public virtual IState Execute()
    {
        if (temp)
        {
            //byt scene
            SceneManager.LoadScene(0);
        }
        return null;
    }
    public virtual IState Exit()
    {
        return null;
    }
}
