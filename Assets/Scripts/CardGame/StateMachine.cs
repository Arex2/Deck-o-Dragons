using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class StateMachine : MonoBehaviour
{
    //försöker göra den här till en singleton
    static StateMachine mInstance;

    public static StateMachine Instance
    {
        get
        {
            return mInstance ? (mInstance = (new GameObject("MyClassContainer")).AddComponent<StateMachine>()): mInstance;
        }
    }






    [SerializeField]
    GameBehaviour gameBehaviour;
    IState activeState = new SetupState(); //state man bör börja med

    // Start is called before the first frame update
    void Start()
    {
        activeState.Enter(gameBehaviour);
        //ChangeState(newState);
    }

    // Update is called once per frame
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
        

        //newState = null;
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
