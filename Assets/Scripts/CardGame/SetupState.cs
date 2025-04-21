using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupState : IState //VET EJ OM MONO BEH�VS H�R, ALTERNATIVT HA DEN I ISTATE
{

    GameBehaviour gameBehaviour;
    public virtual IState Enter(GameBehaviour gameBehaviour)
    {
        Debug.Log("SETUP!!");
        this.gameBehaviour = gameBehaviour;
        gameBehaviour.NewTurn();
        gameBehaviour.UpdateStatusText("New turn");
        return null; 
    }

    public virtual IEnumerator PlayEffects(GameBehaviour gameBehaviour)
    {
        yield return new WaitForSeconds(1.5f);

    }

    public virtual IState Execute()
    {
        Debug.Log("reset mana");
        gameBehaviour.ResetMana();
        if (gameBehaviour.cardHand.CardsInHand.Count > 0)
        {
            return new DiscardState();
        }

        gameBehaviour.NewTurn();
        gameBehaviour.ResetEndTurnButtonText();

        Target.TurnStart.Invoke(Team.Player);

        Debug.Log("discard old cards");
        gameBehaviour.cardHand.cardBeingPlayed = null;
        gameBehaviour.cardHand.EmptyHand();
        Debug.Log("draw new cards");
        //gameBehaviour.cardHand.DrawNewHand();
        gameBehaviour.StartCoroutine(gameBehaviour.cardHand.DrawNewHandNew());
        return new SelectionState(); //byter till selection State efter det h�r
    }
    public virtual IState Exit()
    {
        return null;
    }
}
