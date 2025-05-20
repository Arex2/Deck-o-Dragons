using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLockCardHand : TutorialObject
{
    [SerializeField] private CardHand cardHand;

    public override void Enable()
    {
        cardHand.LockedInTutorial = false;
    }

    public override void Disable()
    {
        cardHand.LockedInTutorial = true;
    }
}
