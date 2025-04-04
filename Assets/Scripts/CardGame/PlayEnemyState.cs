using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayEnemyState : IState
{
    public virtual IState Enter()
    {
        Debug.Log("ENEMY!!");
        //play own cards
        return null;
    }


    public virtual IState Execute()
    {
        //do nothing
        return null;
    }

    public virtual IState Exit()
    {
        return null;
    }
}
