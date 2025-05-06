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

        gameBehaviour.EnableButton();
        gameBehaviour.ResetButtonText();
        gameBehaviour.CheckCardAvailability();
        //gameBehaviour.indicatorManager.ClearIndicators();
        gameBehaviour.indicatorManager.UpdateIndicatorsForOldCard();

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
        //Added check here since enemy now can die after card has finished playing - Harriet 
        BattleOverState battleOver = BattleOverState.BattleOverCheck();
        if (battleOver != null)
        {
            return new BattleOverState();
        }

        //if endTurn button pressed end turn
        if (gameBehaviour.ButtonPressed)
        {
            Target.TurnEnd.Invoke(Team.Player);

            if (gameBehaviour.cardHand.DoDiscardState)
            {
                return new DiscardState();
            }
            else return new PlayEnemyState();
        }

        /*
        //recieve input and select card
        //kolla om card är kort som ska selecta mer, gå då till selectAdditionalCard state maybe?
        if (gameBehaviour.cardHand.IsPlayingCard && gameBehaviour.cardHand.CardBeingPlayed.AffectOtherCards)
        {
            return new SelectAdditionalCardsState();
        }
        else 
        */
        if (gameBehaviour.cardHand.IsPlayingCard)//gameBehaviour.cardHand.cardBeingPlayed != null)
        {
            return new PlayCardState();
        }


        return null;
    }

    public virtual IState Exit()
    {
        gameBehaviour.DisableButton();
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
