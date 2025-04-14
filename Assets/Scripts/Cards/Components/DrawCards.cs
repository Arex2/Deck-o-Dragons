using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCards : CardComponent, IUse
{
    [SerializeField] private Card[] cardsToDraw;

    public void Use()
    {
        // TODO: ADD OPTION TO DRAW CARDS FROM AN ARRAY FIELD
        Debug.LogWarning("There is currently no functionality for drawing a specific card in CardHand!");
    }
}
