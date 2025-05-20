using System.Collections;

public interface IState
{
    public void Enter(GameBehaviour gameBehaviour);

    public IState Execute();

    public void Exit();

    public IEnumerator Coroutine()
    {
        return null;
    }
}
