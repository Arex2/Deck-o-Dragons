using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayCardState : IState
{
    GameBehaviour gameBehaviour;
    CardHand cardHand;
    ControlsV2 controls;
    public virtual IState Enter(GameBehaviour gameBehaviour)
    {
        Debug.Log("PLAY CARD!!");
        this.gameBehaviour = gameBehaviour;
        cardHand = gameBehaviour.cardHand;
        controls = gameBehaviour.controls;

        gameBehaviour.UpdateStatusText("Card playing");

        controls.controls.Enable();
        return null;
    }

    public virtual IEnumerator PlayEffects(GameBehaviour gameBehaviour)
    {
        return null;
    }

    public virtual IState Execute()
    {
        if (!cardHand.IsPlayingCard)
        {
            //gameBehaviour.EnemyTakeDamage(5);  //DET H�R H�NDER VARJE FRAME HELA TIDEN

            //when played effect is done // could possibly be a cooldown timer have timer in gameBehaviour and return? would that work?
            //check enemy hp,
            //if enemy hp <= 0 return gameWon
            //else return selectionState

            BattleOverState battleOver = BattleOverState.BattleOverCheck();

            if (battleOver != null)
            {
                return new BattleOverState();
            }
            else
            {
                return new SelectionState();
            }
        }
        else if (cardHand.CardBeingPlayed != null && cardHand.CardBeingPlayed.WaitingForCardsToAffect)
        {
            return new SelectCardsToAffectState();
        }

        return null;
    }

    public virtual IState Exit()
    {
        controls.controls.Disable();
        return null;
    }
}
