using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    [SerializeField]
    GameBehaviour gameBehaviour;
    IState activeState = new SetupState(); //state man bör börja med

    public IState ActiveState => activeState;

    void Start()
    {
        activeState.Enter(gameBehaviour);
    }

    void Update()
    {
        if (activeState != null)
        {
            var newState = activeState.Execute();

            //if return is not null aka is a new state, change state
            if(newState != null && newState != activeState)
                ChangeState(newState);
        }
    }

    private void ChangeState(IState newState)
    {
        if (activeState != null)
        {
            activeState.Exit();
        }
        activeState = newState;
        activeState.Enter(gameBehaviour);
    }




    /*
    protected IState State;

    public void SetState(IState state)
    {
        State = state;
        StartCoroutine(State.Start());
    }


    */
}
