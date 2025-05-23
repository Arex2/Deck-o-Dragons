using System.Collections;
using UnityEngine;

public class DiscardState : IState
{
    private GameBehaviour gameBehaviour;
    private ControlsV2 controls;

    private bool _switchState;

    private CardFilterResult DiscardFilter(CardObject cardObject)
    {
        if (cardObject.HasTag(CardManager.BindingTag))
        {
            return CardFilterResult.Failure("Binding cards can't be discarded.");
        }
        if (cardObject.TryGetTagPotency(CardManager.VanishingTag, out float potency))
        {
            if (potency <= 1)
            {
                return CardFilterResult.Failure("This card will vanish next turn.");
            }
        }

        return CardFilterResult.Success();
    }

    public void Enter(GameBehaviour gameBehaviour)
    {
        Debug.Log("DISCARDING CARDS");
        this.gameBehaviour = gameBehaviour;
        controls = gameBehaviour.controls;

        gameBehaviour.cardHand.OnStartSelectingCards(CardHand.SelectionState.Discard, null, null, DiscardFilter);
        //gameBehaviour.UpdateStatusText("DISCARDING CARDS");

        gameBehaviour.EnableButton();

        gameBehaviour.StatusButton.ProceedStatus(CardgameStatusButton.DISCARD, CardgameStatusButton.ENEMY_TURN);

        controls.controls.Enable();
    }

    public IState Execute()
    {
        if (gameBehaviour.ButtonPressed)
        {
            gameBehaviour.cardHand.EmptyHand();
            gameBehaviour.cardHand.OnExitSelectingCards();

            controls.controls.Disable();

            gameBehaviour.DisableButton();

            gameBehaviour.StartCoroutine(Delay());

            gameBehaviour.cardHand.UpdateCardPositions();

            gameBehaviour.StatusButton.ProceedStatus(CardgameStatusButton.ENEMY_TURN, CardgameStatusButton.PLAYER_TURN);
            gameBehaviour.cardHand.CanPlayCards = false;
        }

        if (_switchState && CardVFXManager.ActiveVFXCount <= 0)
        {
            if (BattleOverState.BattleOverCheck(out Team? winningTeam))
            {
                return new BattleOverState(winningTeam);
            }

            return new PlayEnemyState();
        }

        return null;
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(1);

        gameBehaviour.cardHand.UpdateCardPositions();

        _switchState = true;
    }

    public void Exit()
    {
        controls.controls.Disable();
    }
}
