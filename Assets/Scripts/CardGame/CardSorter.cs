using System.Collections.Generic;

/// <summary>
/// Sorts both <see cref="Card"/> and <see cref="CardObject"/> by their mana cost and name.
/// </summary>
public class CardSorter : IComparer<CardObject>, IComparer<Card>
{
    public static readonly CardSorter Instance = new();

    public int Compare(CardObject a, CardObject b)
    {
        int manaComparison = a.GetCost().CompareTo(b.GetCost());

        if (manaComparison != 0 || a.Card == null || b.Card == null)
        {
            return manaComparison;
        }

        int nameComparison = a.Card.DisplayName.CompareTo(b.Card.DisplayName);

        if (nameComparison != 0)
        {
            return nameComparison;
        }

        return a.TimeCreated.CompareTo(b.TimeCreated);
    }

    public int Compare(Card a, Card b)
    {
        int manaComparison = a.Cost.CompareTo(b.Cost);

        if (manaComparison != 0)
        {
            return manaComparison;
        }

        return a.DisplayName.CompareTo(b.DisplayName);
    }
}
