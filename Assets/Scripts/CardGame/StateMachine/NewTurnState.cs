using UnityEngine;

public class NewTurnState : IState //VET EJ OM MONO BEH�VS H�R, ALTERNATIVT HA DEN I ISTATE
{
    GameBehaviour gameBehaviour;
    CardHand cardHand;
    public void Enter(GameBehaviour gameBehaviour)
    {
        Debug.Log("SETUP!!");
        this.gameBehaviour = gameBehaviour;
        cardHand = gameBehaviour.cardHand;
        //gameBehaviour.UpdateStatusText("New turn");
    }

    public IState Execute()
    {
        Debug.Log("reset mana");
        gameBehaviour.ResetMana();

        Target.TurnStart.Invoke(Team.Player);

        gameBehaviour.StartCoroutine(cardHand.DrawNewHand());

        return new SelectionState(); //byter till selection State efter det h�r
    }

    public void Exit()
    {

    }
}
