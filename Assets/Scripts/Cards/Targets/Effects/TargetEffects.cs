using UnityEngine;

public class TargetEffects : MonoBehaviour, ITargetCallbacks
{
    private Target _target;

    private void Awake()
    {
        _target = GetComponentInParent<Target>(true);
    }

    public void OnHurt()
    {

    }

    public void OnDeath()
    {

    }

    public void OnHeal()
    {

    }

    public void OnUpdateHP()
    {

    }

    public void OnUpdateStatusEffects()
    {

    }

    public void OnTurnStart()
    {

    }

    public void OnTurnEnd()
    {

    }
}
