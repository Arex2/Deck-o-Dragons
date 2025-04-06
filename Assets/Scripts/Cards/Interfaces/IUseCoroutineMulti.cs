using System.Collections;
using System.Collections.Generic;

public interface IUseCoroutineMulti
{
    public IEnumerator UseCoroutine(List<Target> targets);
}
