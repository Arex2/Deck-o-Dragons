using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class StateMachine : MonoBehaviour
{
    IState activeState = new SetupState(); //state man bör börja med

    // Start is called before the first frame update
    void Start()
    {
        activeState.Enter();
        //ChangeState(newState);
    }

    // Update is called once per frame
    void Update()
    {
        if (activeState != null)
        {
            var newState = activeState.Execute();

            //if return is not null aka is a new state, change state
            if(newState != null)
                ChangeState(newState);
        }
    }

    private void ChangeState(IState newState)
    {
        if (newState != null)
        {
            activeState.Exit();
        }
        activeState = newState;
        activeState.Enter();
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
