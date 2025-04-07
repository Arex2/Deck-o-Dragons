using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    /*
    public virtual IEnumerator Start() 
    {
        yield break;
    }
    */

    public virtual IState Enter(GameBehaviour gameBehaviour)
    {
        return null;
    }


    // Update is called once per frame
    public virtual IState Execute()
    {
        return null;
    }

    public virtual IState Exit()
    {
        return null;
    }
}
