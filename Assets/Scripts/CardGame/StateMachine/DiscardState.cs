using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardState : IState
{
    private GameBehaviour gameBehaviour;
    private ControlsV2 controls;

    private bool _switchState;

    public virtual IState Enter(GameBehaviour gameBehaviour)
    {
        Debug.Log("DISCARDING CARDS AND CHOOSING WHICH TO KEEP");
        this.gameBehaviour = gameBehaviour;
        controls = gameBehaviour.controls;

        gameBehaviour.cardHand.OnStartSelectingCards(CardHand.SelectionState.Discard);
        gameBehaviour.UpdateStatusText("DISCARDING CARDS");

        gameBehaviour.EnableButton();
        gameBehaviour.SetButtonText("Done");

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
            gameBehaviour.cardHand.OnExitSelectingCards();

            controls.controls.Disable();

            gameBehaviour.DisableButton();

            gameBehaviour.StartCoroutine(Delay());
        }

        if (_switchState)
        {
            return new PlayEnemyState();
        }

        return null;
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(1);

        _switchState = true;
    }

    public virtual IState Exit()
    {
        return null;
    }
}
