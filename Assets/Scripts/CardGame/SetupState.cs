using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupState : IState //VET EJ OM MONO BEHÖVS HÄR, ALTERNATIVT HA DEN I ISTATE
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


    public virtual IState Execute()
    {
        Debug.Log("reset mana");
        gameBehaviour.ResetMana();
        Debug.Log("discard old cards");
        gameBehaviour.cardHand.EmptyHand();
        Debug.Log("draw new cards");
        gameBehaviour.cardHand.DrawNewHand();
        return new SelectionState(); //byter till selection State efter det här
    }
    public virtual IState Exit()
    {
        return null;
    }
}
