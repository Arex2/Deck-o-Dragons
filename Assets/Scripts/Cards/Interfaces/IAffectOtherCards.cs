using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Put this interface on your <see cref="CardComponent"/> to make it confess that it does affect other cards in some way.
/// </summary>
// Script by Ruben
public interface IAffectOtherCards
{
    /// <summary>
    /// The limit of how many cards can be selected. Set this to 0 or null for no limit.
    /// </summary>
    public int? Count { get; }

    /// <summary>
    /// If this is true then the EXACT amount of cards in <see cref="Count"/> will be required in order for this card to execute. <para/>
    /// The player will get the option to "Cancel" a card with this set to true to prevent any softlocks. Pressing "Cancel" also makes the card stop itself.
    /// </summary>
    public bool RequireExactAmount { get; }

    /// <summary>
    /// What to display when selecting cards to affect. Put {0} in your string to display how many are selected and {1} to display the maximum selectable amount. <para/>
    /// <b>Example:</b> <c>"Select up to {1} cards to explode. ({0}/{1})"</c>. <para/>
    /// Alternatively, return null to make the select message auto set itself to a generic one.
    /// </summary>
    public string SelectMessage { get; }

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
