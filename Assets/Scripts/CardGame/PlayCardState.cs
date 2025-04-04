using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCardState : IState
{
    public virtual IState Enter()
    {
        Debug.Log("PLAY CARD!!");
        return null;
    }


    public virtual IState Execute()
    {
        //if endturn button input
        if(false)
            return new PlayEnemyState();

        return null;
    }

    public virtual IState Exit()
    {
        return null;
    }
}
