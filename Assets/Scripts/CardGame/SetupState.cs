using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupState : IState //VET EJ OM MONO BEHÖVS HÄR, ALTERNATIVT HA DEN I ISTATE
{
    private Controls controls;
    public virtual IState Enter(Controls controls)
    {
        this.controls = controls;
        //reset mana
        //discard old cards (?)
        //draw new cards
        Debug.Log("SETUP!!");
        return null; 
    }


    public virtual IState Execute()
    {
        Debug.Log("reset mana");
        Debug.Log("discard old cards");
        Debug.Log("draw new cards");
        return new SelectionState(); //byter till selection State efter det här
    }
    public virtual IState Exit()
    {
        return null;
    }
}
