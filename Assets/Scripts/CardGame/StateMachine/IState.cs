using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{

    public virtual IState Enter(GameBehaviour gameBehaviour)
    {
        return null;
    }

    public virtual IEnumerator PlayEffects(GameBehaviour gameBehaviour)
    {
        return null;
    }

    public virtual IState Execute()
    {
        return null;
    }

    public virtual IState Exit()
    {
        return null;
    }
}
