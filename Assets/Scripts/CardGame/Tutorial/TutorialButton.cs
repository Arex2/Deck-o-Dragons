using System;
using System.Collections;
//using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;

public class TutorialButton : TutorialObject
{
    private CanvasGroup _canvasGroup;

    private bool _active;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        if (_canvasGroup != null)
        {
            _canvasGroup.blocksRaycasts = false;
        }

        foreach (Button button in GetComponentsInChildren<Button>(true))
        {
            button.onClick.AddListener(Trigger);
        }
    }

    public override void Enable()
    {
        _active = false;

        StartCoroutine(Delay());

        if (_canvasGroup != null)
        {
            _canvasGroup.blocksRaycasts = true;
        }
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.5f);

        _active = true;
    }

    public override void Disable()
    {
        _active = false;

        if (_canvasGroup != null)
        {
            _canvasGroup.blocksRaycasts = false;
        }
    }

    public void Trigger()
    {
        if (!_active)
        {
            return;
        }

        CardgameTutorialManager.ProgressTutorial();
    }
}
