using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectAdditionalCardState : IState
{
    private GameBehaviour gameBehaviour;
    private ControlsV2 controls;

    public virtual IState Enter(GameBehaviour gameBehaviour)
    {
        Debug.Log("Selecting card(s) to affect");
        this.gameBehaviour = gameBehaviour;
        this.controls = gameBehaviour.controls;
        //gameBehaviour.NewTurn();
        gameBehaviour.SetEndTurnButtonText("Done");
        gameBehaviour.cardHand.OnEnterSelectAdditionalCardState();
        gameBehaviour.UpdateStatusText("SELECT CARD TO AFFECT");

        controls.controls.Enable();

        return null;
    }

    public virtual IEnumerator PlayEffects(GameBehaviour gameBehaviour)
    {
        return null;
    }

    public virtual IState Execute()
    {
        if (gameBehaviour.CardSelected)
        {

            gameBehaviour.CardSelected = false;
            gameBehaviour.NewTurn();
            //gameBehaviour.cardHand.cardBeingPlayed = null;
            //gameBehaviour.cardHand.EmptyHand();

            return new PlayCardState();
        }

        return null;
    }

    public virtual IState Exit()
    {

        gameBehaviour.cardHand.PlayCard(gameBehaviour.cardHand.cardBeingPlayed);
        //Target.TurnStart.Invoke(Team.Player);

        gameBehaviour.ResetEndTurnButtonText();

        //gameBehaviour.StartCoroutine(gameBehaviour.cardHand.DrawNewHandNew());
        gameBehaviour.cardHand.OnExitSelectAdditionalCardState();

        controls.controls.Disable();
        return null;
    }
}
