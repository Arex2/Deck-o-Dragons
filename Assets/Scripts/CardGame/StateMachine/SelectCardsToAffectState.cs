using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCardsToAffectState : IState
{
    private GameBehaviour gameBehaviour;
    private CardHand cardHand;
    private ControlsV2 controls;
    private CardObject cardObject;

    private bool requireExactAmount;
    private bool cancelled;

    public virtual IState Enter(GameBehaviour gameBehaviour)
    {
        Debug.Log("Selecting card(s) to affect");
        this.gameBehaviour = gameBehaviour;
        controls = gameBehaviour.controls;
        cardHand = gameBehaviour.cardHand;
        //gameBehaviour.NewTurn();

        cardObject = cardHand.CardBeingPlayed;
        IAffectOtherCardsHandler affectOtherCards = cardObject.Card.CurrentComponent as IAffectOtherCardsHandler;
        int? count = affectOtherCards.Count;
        requireExactAmount = affectOtherCards.RequireExactAmount;

        cardHand.OnStartSelectingCards(CardHand.SelectionState.AffectCards, count.HasValue ? count.Value : 0, affectOtherCards.SelectMessage, affectOtherCards.FilterCardObject);
        gameBehaviour.UpdateStatusText("SELECT CARD TO AFFECT");

        controls.controls.Enable();

        if (requireExactAmount)
        {
            gameBehaviour.SetButtonText("Cancel");
        }
        else
        {
            gameBehaviour.SetButtonText("Done");
        }

        gameBehaviour.EnableButton();

        return null;
    }

    public virtual IEnumerator PlayEffects(GameBehaviour gameBehaviour)
    {
        return null;
    }

    public virtual IState Execute()
    {
        if (requireExactAmount)
        {
            if (cardHand.SelectedCardsCount < cardHand.CurrentSelectedCardsLimit)
            {
                gameBehaviour.SetButtonText("Cancel");
            }
            else
            {
                gameBehaviour.SetButtonText("Done");
            }
        }

        if (gameBehaviour.ButtonPressed)
        {
            if (requireExactAmount)
            {
                cancelled = cardHand.SelectedCardsCount < cardHand.CurrentSelectedCardsLimit;
            }

            return new PlayCardState();
        }

        return null;
    }

    public virtual IState Exit()
    {

        //gameBehaviour.cardHand.PlayCard(gameBehaviour.cardHand.CardBeingPlayed);
        //Target.TurnStart.Invoke(Team.Player);

        if (cancelled)
        {
            cardObject.Cancel();
        }
        else
        {
            foreach (CardObject cardObj in cardHand.SelectedCards)
            {
                cardObject.CardsToAffect.Add(cardObj);
            }
        }

        cardObject.Card.FinishedSettingCardsToAffect();

        gameBehaviour.DisableButton();

        //gameBehaviour.StartCoroutine(gameBehaviour.cardHand.DrawNewHandNew());
        cardHand.OnExitSelectingCards();

        controls.controls.Disable();
        return null;
    }
}
