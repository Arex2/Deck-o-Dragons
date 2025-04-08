using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardHand : CardComponent, IUse
{
    public void Use()
    {
        // TODO: Temporarily no method to only discard hand yet :(
        CardHand hand = FindObjectOfType<CardHand>();

        hand.DrawNewHand();
    }
}
