using System.Collections;
using UnityEngine;

public class BattleOverState : IState
{
    bool coroutineOver = false; //ANV�NDS F�R ATT K�NNA N�R WAITTIME �R DONE

    GameBehaviour gameBehaviour;
    private ControlsV2 controls;
    private bool loadingScene;
    private Team? winningTeam;

    public BattleOverState() { }

    public BattleOverState(Team? winningTeam) : this()
    {
        this.winningTeam = winningTeam;
    }

    public void Enter(GameBehaviour gameBehaviour)
    {
        Debug.Log("BATTLE OVER");
        this.gameBehaviour = gameBehaviour;
        controls = gameBehaviour.controls;
        //gameBehaviour.NewTurn();
        //gameBehaviour.UpdateStatusText("BATTLE OVER");

        //stäng av controls
        controls.controls.Disable();
    }

    public IEnumerator Coroutine()
    {
        yield return new WaitForSeconds(1f);

        gameBehaviour.BattleOver(winningTeam.HasValue ? (winningTeam.Value == Team.Player ? "You won!" : "You lost...") : "It's a draw...");

        //Debug.Log("Wait start " + Time.time);
        yield return new WaitForSeconds(2f);
        //Debug.Log("Wait over " + Time.time);
        coroutineOver = true;
    }

    public IState Execute()
    {
        if (coroutineOver && !loadingScene)
        {
            //Debug.Log("New Encounter");
            //gameBehaviour.NewEncounter();

            //WIN
            if (winningTeam.HasValue && winningTeam.Value == Team.Player)
            {
                // Card shop scene
                SceneSwitcher.SwitchToCardShop();
            }
            //LOSE
            else
            {
                //end game
                //åk tillbaka till ägg scenen
                SceneSwitcher.SwitchToEgg();
            }

            loadingScene = true;
        }
        return null;
    }

    public void Exit()
    {

    }

    public static bool BattleOverCheck(out Team? winningTeam)
    {
        Team? aliveTeam = null;

        foreach (Target target in TargetManager.AllTargets)
        {
            if (target.Dead)
            {
                continue;
            }

            Team team = target.Team;

            // There are multiple teams alive. All need to be dead except one in order for a battle to be over
            if (aliveTeam.HasValue && aliveTeam.Value != team)
            {
                winningTeam = null;
                return false;
            }

            // This is the team that is alive
            aliveTeam = team;
        }

        if (aliveTeam.HasValue)
        {
            winningTeam = aliveTeam.Value;

            return true;
        }

        // All teams are dead
        winningTeam = null;
        return true;
    }
}
