using System.Collections;

/// <summary>
/// Implement this interface onto your component to make get callbacks from the <see cref="CardVFX"/> component. <para/>
/// When the <see cref="VFXCoroutine"/> method to ends, then the <see cref="CardVFX"/> will end (this is shared between multiple <see cref="ICardVFXComponent"/>s).
/// </summary>
public interface ICardVFXComponent 
{
    public void OnVFXCreated(CardVFX cardVFX);

    public IEnumerator VFXCoroutine(CardVFX cardVFX);

    public void OnVFXDestroyed(CardVFX cardVFX);
}
