using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.PlayerLoop;

public class SelectionState : IState
{
    //[SerializeField]
    bool test = false;
    protected Controls controls;
    public virtual IState Enter(Controls controls)
    {
        this.controls = controls;
        Debug.Log("SELECTION!!");
        //player gets input

        controls.controls.Enable();
        return null;
    }


    public virtual IState Execute()
    {
        //recieve input and select card
        //if play selected card
        if (test)
            return new PlayCardState();

        return null;
    }

    public virtual IState Exit()
    {
        controls.controls.Disable();
        return null;
    }

}
