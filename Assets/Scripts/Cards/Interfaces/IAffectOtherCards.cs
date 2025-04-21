using System.Collections;

/// <summary>
/// This interface confesses that it does affect other cards.
/// </summary>
public interface IAffectOtherCards
{
    public int Count { get; }

    public IEnumerator OnCardsSelected(CardObject[] cardObjects);
}
