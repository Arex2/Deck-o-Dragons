using System.Collections;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    [SerializeField]
    GameBehaviour gameBehaviour;
    IState activeState = new IntroState(); //state man bör börja med

    public IState ActiveState => activeState;

    void Start()
    {
        EnterState(activeState);
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
        EnterState(activeState);
    }

    private void EnterState(IState state)
    {
        state.Enter(gameBehaviour);

        IEnumerator coroutine = state.Coroutine();

        if (coroutine != null)
        {
            StartCoroutine(coroutine);
        }
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
