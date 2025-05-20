using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCardHand : TutorialObject
{
    [SerializeField] private CardHand cardHand;

    private bool _active;
    private bool _progress;

    public override void Enable()
    {
        _active = true;
        _progress = false;
    }

    public override void Disable()
    {
        _active = false;
    }

    private void Update()
    {
        if (!_active || _progress)
        {
            return;
        }

        if (cardHand.CardBeingPlayed != null)
        {
            _progress = true;

            CardgameTutorialManager.ProgressTutorial();
        }
    }
}
