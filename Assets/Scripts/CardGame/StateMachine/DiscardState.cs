using System.Collections;
using System.Collections.Generic;
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
        if (cardObject.HasTag(CardManager.SlipperyTag))
        {
            return CardFilterResult.Failure("Slippery cards will be auto-discarded.");
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

    public virtual IState Enter(GameBehaviour gameBehaviour)
    {
        Debug.Log("DISCARDING CARDS");
        this.gameBehaviour = gameBehaviour;
        controls = gameBehaviour.controls;

        gameBehaviour.cardHand.OnStartSelectingCards(CardHand.SelectionState.Discard, null, null, DiscardFilter);
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

            gameBehaviour.cardHand.UpdateCardPositions();
        }

        if (_switchState)
        {
            BattleOverState battleOverState = BattleOverState.BattleOverCheck();

            if (battleOverState != null)
            {
                return battleOverState;
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

    public virtual IState Exit()
    {
        controls.controls.Disable();
        return null;
    }
}
