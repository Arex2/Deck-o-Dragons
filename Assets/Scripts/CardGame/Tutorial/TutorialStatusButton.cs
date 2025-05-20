using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialStatusButton : TutorialObject
{
    [SerializeField] private GameBehaviour gameBehaviour;
    private CardgameStatusButton _button;
    private bool _active;

    private void Start()
    {
        _button = gameBehaviour.StatusButton;
        _button.ForcedOff = true;
    }

    public override void Enable()
    {
        _button.ForcedOff = false;
        _active = true;
    }

    public override void Disable()
    {
        _button.ForcedOff = true;
        _active = false;
    }

    private void Update()
    {
        if (!_active)
        {
            return;
        }

        if (gameBehaviour.ButtonPressed)
        {
            CardgameTutorialManager.ProgressTutorial();
        }
    }

    public override void OnTutorialOver()
    {
        _button.ForcedOff = false;
    }
}
