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
        controls = gameBehaviour.controls;
        gameBehaviour.EnableButton();
        gameBehaviour.SetButtonText("Done");
        gameBehaviour.cardHand.OnStartSelectingCards(CardHand.SelectionState.Discard);
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
        if (gameBehaviour.ButtonPressed)
        {
            gameBehaviour.cardHand.EmptyHand();

            return new SelectionState();
        }

        return null;
    }

    public virtual IState Exit()
    {
        Target.TurnStart.Invoke(Team.Player);

        gameBehaviour.EnableButton();
        gameBehaviour.ResetButtonText();

        gameBehaviour.StartCoroutine(gameBehaviour.cardHand.DrawNewHand());
        gameBehaviour.cardHand.OnExitSelectingCards();

        controls.controls.Disable();
        return null;
    }
}
