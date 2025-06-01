using UnityEngine;

public class PlayCardState : IState
{
    GameBehaviour gameBehaviour;
    CardHand cardHand;
    ControlsV2 controls;
    public void Enter(GameBehaviour gameBehaviour)
    {
        Debug.Log("PLAY CARD!!");
        this.gameBehaviour = gameBehaviour;
        cardHand = gameBehaviour.cardHand;
        controls = gameBehaviour.controls;

        //gameBehaviour.UpdateStatusText("Card playing");

        controls.controls.Enable();
    }

    public IState Execute()
    {
        if (!cardHand.IsPlayingCard)
        {
            if (BattleOverState.BattleOverCheck(out Team? winningTeam))
            {
                return new BattleOverState(winningTeam);
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

    public void Exit()
    {
        controls.controls.Disable();
    }
}
