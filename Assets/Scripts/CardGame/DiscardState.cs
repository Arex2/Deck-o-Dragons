using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardState : IState
{
    private GameBehaviour gameBehaviour;
    private ControlsV2 controls;

    public virtual IState Enter(GameBehaviour gameBehaviour)
    {
        Debug.Log("DISCARDING CARDS AND CHOOSING WHICH TO KEEP");
        this.gameBehaviour = gameBehaviour;
        this.controls = gameBehaviour.controls;
        gameBehaviour.NewTurn();
        gameBehaviour.SetEndTurnButtonText("Done");
        gameBehaviour.cardHand.OnEnterDiscardState();
        gameBehaviour.UpdateStatusText("DISCARDING CARDS");

        controls.controls.Enable();

        return null;
    }

    public virtual IEnumerator PlayEffects(GameBehaviour gameBehaviour)
    {
        return null;
    }

    public virtual IState Execute()
    {
        if (gameBehaviour.EndTurn)
        {
            gameBehaviour.NewTurn();

            gameBehaviour.cardHand.cardBeingPlayed = null;
            gameBehaviour.cardHand.EmptyHand();

            return new SelectionState();
        }

        return null;
    }

    public virtual IState Exit()
    {
        Target.TurnStart.Invoke(Team.Player);

        gameBehaviour.ResetEndTurnButtonText();

        gameBehaviour.StartCoroutine(gameBehaviour.cardHand.DrawNewHandNew());
        gameBehaviour.cardHand.OnExitDiscardState();

        controls.controls.Disable();
        return null;
    }
}
