using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardgameTutorialManager : Singleton<CardgameTutorialManager>
{
    public static readonly List<TutorialObject> TutorialObjects = new();

    public static bool InTutorial => Instance == null ? false : Instance._inTutorial;

    private bool _inTutorial;

    public static int TutorialStep { get; private set; } = 0;

    public Card[] TutorialCards => tutorialCards;

    [SerializeField] private GameObject activateObj;
    [SerializeField] private Card[] tutorialCards;

    private HashSet<TutorialObject> _activeTutorialObj = new();

    public static void StartTutorial()
    {
        Instance._inTutorial = true;

        Instance.activateObj.SetActive(true);

        TutorialStep = 0;

        Instance.NextStep();
    }

    public static void ProgressTutorial()
    {
        TutorialStep++;

        Instance.NextStep();
    }

    private void NextStep()
    {
        foreach (TutorialObject obj in _activeTutorialObj)
        {
            obj.Disable();
        }

        _activeTutorialObj.Clear();

        bool tutorialOver = true;

        List<TutorialObject> activeOnAll = new();

        foreach (TutorialObject obj in TutorialObjects)
        {
            if (!obj.ActiveOn.Contains(TutorialStep))
            {
                if (obj.ActiveOnAll)
                {
                    activeOnAll.Add(obj);
                }

                continue;
            }

            tutorialOver = false;
            obj.Enable();
            _activeTutorialObj.Add(obj);
        }

        if (!tutorialOver)
        {
            foreach (TutorialObject obj in activeOnAll)
            {
                obj.Enable();
                _activeTutorialObj.Add(obj);
            }

            return;
        }

        // TUTORIAL IS OVER, GO HOME!
        _inTutorial = false;

        foreach (TutorialObject obj in TutorialObjects)
        {
            obj.OnTutorialOver();
        }
    }
}
