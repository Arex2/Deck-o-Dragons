using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <see cref="CardComponent"/> that will discard the players entire hand on use.
/// </summary>
// Script by Ruben
[AddComponentMenu("Hand/Discard Hand")]
public class DiscardHand : CardComponent
{
    private CardHand _cardHand;

    public override void Play(List<Target> targets)
    {
        if (_cardHand == null)
        {
            _cardHand = CardHand.Instance;

            if (_cardHand == null)
            {
                Debug.LogWarning($"There is no {nameof(CardHand)} in the scene!");
                return;
            }
        }

        _cardHand.EmptyHand();
    }
}
