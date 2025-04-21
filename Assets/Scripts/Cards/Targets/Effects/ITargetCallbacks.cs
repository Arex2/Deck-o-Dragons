public interface ITargetCallbacks
{
    public void OnHurt();

    public void OnDeath();

    public void OnHeal();

    public void OnUpdateHP();

    public void OnUpdateStatusEffects();

    public void OnTurnStart();

    public void OnTurnEnd();
}
