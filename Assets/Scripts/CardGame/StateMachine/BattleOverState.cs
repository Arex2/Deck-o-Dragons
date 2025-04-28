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
            /*
            if(gameBehaviour.HP <= 0)
            {
                //end game
                //åk tillbaka till ägg scenen
                gameBehaviour.SwitchToEggScene();
                return null;
            }
            Debug.Log("New Encounter");
            gameBehaviour.NewEncounter();
            */

            // Card shop scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(6);
        }
        return null;
    }
    public virtual IState Exit()
    {
        return null;
    }

    public static BattleOverState BattleOverCheck()
    {
        HashSet<Team> aliveTeams = new();

        foreach (Target target in TargetManager.AllTargets)
        {
            if (target.Dead)
            {
                continue;
            }

            Team team = target.Team;

            if (aliveTeams.Contains(team))
            {
                continue;
            }

            aliveTeams.Add(team);
        }

        foreach (Team team in Teams.AllTeams)
        {
            if (aliveTeams.Contains(team))
            {
                continue;
            }

            return new BattleOverState();
        }

        return null;
    }
}
