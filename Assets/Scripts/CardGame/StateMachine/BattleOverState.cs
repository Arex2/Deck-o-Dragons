using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;


public class BattleOverState : IState
{
    bool coroutineOver = false; //ANV�NDS F�R ATT K�NNA N�R WAITTIME �R DONE

    GameBehaviour gameBehaviour;
    private ControlsV2 controls;
    private bool loadingScene;
    public virtual IState Enter(GameBehaviour gameBehaviour)
    {
        Debug.Log("BATTLE OVER");
        this.gameBehaviour = gameBehaviour;
        controls = gameBehaviour.controls;
        //gameBehaviour.NewTurn();
        //gameBehaviour.UpdateStatusText("BATTLE OVER");

        //stäng av controls
        controls.controls.Disable();

        gameBehaviour.StartCoroutine(Wait()); //s�tter temp till true

        return null;
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1f);

        gameBehaviour.BattleOver(gameBehaviour.HP <= 0 ? "You lost..." : "You won!");

        //Debug.Log("Wait start " + Time.time);
        yield return new WaitForSeconds(2f);
        //Debug.Log("Wait over " + Time.time);
        coroutineOver = true;
    }

    public virtual IEnumerator PlayEffects(GameBehaviour gameBehaviour)
    {
        return null;
    }

    public virtual IState Execute()
    {
        if (coroutineOver && !loadingScene)
        {
            
            //LOSE
            if(gameBehaviour.HP <= 0)
            {
                //end game
                //åk tillbaka till ägg scenen
                SceneSwitcher.SwitchToEgg();
            }
            //Debug.Log("New Encounter");
            //gameBehaviour.NewEncounter();
            //WIN
            else
            {
                // Card shop scene
                SceneSwitcher.SwitchToCardShop();
            }

            loadingScene = true;
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
