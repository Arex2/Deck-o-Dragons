using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Interface for modifying one or multiple <see cref="CardObject"/>s. Used by <see cref="IAffectOtherCardsHandler"/>.
/// </summary>
// Script by Ruben
public interface IAffectOtherCards
{
    /// <summary>
    /// This method gets called when the player has finally decided what cards should be affected. <para/>
    /// This returns <see cref="IEnumerator"/> for coroutine purposes. If you don't want this to be a coroutine, return null and I don't mean "yield return null" just the basic "return null".
    /// </summary>
    public IEnumerator OnCardsSelected(List<CardObject> cardObjects);

    /// <summary>
    /// Optional to implement, but allows you to filter out if you should be able to select certain <see cref="CardObject"/>s or not using <see cref="CardFilterResult"/>. <para/>
    /// To clarify, if a <see cref="CardObject"/> should be selectable, use <see cref="CardFilterResult.Success"/>. <para/>
    /// Use <see cref="CardFilterResult.Failure"/> if the <see cref="CardObject"/> shouldn't be selectable.
    /// </summary>
    public CardFilterResult FilterCardObject(CardObject cardObject)
    {
        return CardFilterResult.Success();
    }
}
