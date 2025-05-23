using System.Collections.Generic;

/// <summary>
/// Sorts both <see cref="Card"/> and <see cref="CardObject"/> by their mana cost and name.
/// </summary>
public class CardSorter : IComparer<CardObject>, IComparer<Card>
{
    public static readonly CardSorter Instance = new();

    public int Compare(CardObject a, CardObject b)
    {
        int costComparison = CompareCardCost(a.Card, b.Card, a.Cost, b.Cost);

        if (costComparison != 0)
        {
            return costComparison;
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
        int costComparison = CompareCardCost(a, b);

        if (costComparison != 0)
        {
            return costComparison;
        }

        return a.DisplayName.CompareTo(b.DisplayName);
    }

    public int CompareCardCost(Card a, Card b, int? manaCostA = null, int? manaCostB = null)
    {
        if (a == null && b == null)
        {
            return 0;
        }
        else if (a != null && b == null)
        {
            return 1;
        }
        else if (a == null && b != null)
        {
            return -1;
        }

        int GetCost(int cost, Card card, out bool useSelfDmg)
        {
            if (cost <= 0)
            {
                int? selfDamageCost = card.GetCardSelfDamageCost();
                useSelfDmg = selfDamageCost.HasValue;

                if (useSelfDmg)
                {
                    cost = selfDamageCost.Value;
                }
            }
            else
            {
                useSelfDmg = false;
            }

            return cost;
        }

        int costA = GetCost(manaCostA.HasValue ? manaCostA.Value : a.Cost, a, out bool useSelfDmgA);
        int costB = GetCost(manaCostB.HasValue ? manaCostB.Value : b.Cost, b, out bool useSelfDmgB);

        if (useSelfDmgA && !useSelfDmgB)
        {
            return 1;
        }
        else if (!useSelfDmgA && useSelfDmgB)
        {
            return -1;
        }
        else
        {
            return costA.CompareTo(costB);
        }
    }
}
