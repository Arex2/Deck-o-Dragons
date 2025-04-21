using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SelectionState : IState
{
    protected ControlsV2 controls;
    GameBehaviour gameBehaviour;
    public virtual IState Enter(GameBehaviour gameBehaviour)
    {
        this.gameBehaviour = gameBehaviour;
        this.controls = gameBehaviour.controls;
        Debug.Log("SELECTION!!");
        gameBehaviour.UpdateStatusText("Select a card");
        //player gets input
        //Wait();
        controls.controls.Enable();
        return null;
    }

    public virtual IEnumerator PlayEffects(GameBehaviour gameBehaviour)
    {
        return null;
    }

    public virtual IState Execute()
    {
        //if endTurn button pressed end turn
        if (gameBehaviour.EndTurn)
        {
            Target.TurnEnd.Invoke(Team.Player);

            return new PlayEnemyState();
        }

        //recieve input and select card
        //kolla om card är kort som ska selecta mer, gå då till selectAdditionalCard state maybe?
        if(gameBehaviour.cardHand.cardIsPlaying && gameBehaviour.cardHand.cardBeingPlayed.AffectOtherCards)
        {
            return new SelectAdditionalCardState();
        }
        else if (gameBehaviour.cardHand.cardIsPlaying)//gameBehaviour.cardHand.cardBeingPlayed != null)
        {
            return new PlayCardState();
        }


        return null;
    }

    public virtual IState Exit()
    {
        controls.controls.Disable();
        return null;
    }

    /*
    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(5);
        test = true;
    }
    */

}
