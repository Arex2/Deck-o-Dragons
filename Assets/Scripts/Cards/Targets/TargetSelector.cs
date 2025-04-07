using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Responsible for bringing up the select menu for 
/// </summary>
[SingletonMode(true)]
public class TargetSelector : Singleton<TargetSelector>
{
    public static Target TargetResult { get; private set; } = null;
    public static Team? TeamResult { get; private set; } = null;

    [CacheComponent]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image panel;
    private float _panelAlpha;

    [Space]
    [SerializeField] private TargetSelectorButton buttonTemplate;
    [SerializeField] private int startingCount = 2;

    [Space]
    [SerializeField] private CanvasGroup teamSelectGroup;
    [SerializeField] private Button playerTeamButton;
    [SerializeField] private Button enemyTeamButton;

    [Space]
    [SerializeField] private float fadeTime;

    private static bool _chosen = false;

    private static List<TargetSelectorButton> _buttons = new();
    private static int _buttonsCount;

    public Camera Camera
    {
        get
        {
            if (_camera == null)
            {
                _camera = Camera.main;

                if (_camera == null)
                {
                    _camera = FindObjectOfType<Camera>();
                }
            }

            return _camera;
        }
    }
    private Camera _camera;

    public Vector2 ScaleFactor => transform.localScale;

    protected override void Awake()
    {
        base.Awake();

        Color color = panel.color;
        _panelAlpha = color.a;

        color.a = 0;
        panel.color = color;

        teamSelectGroup.alpha = 0;
        teamSelectGroup.blocksRaycasts = false;

        canvasGroup.blocksRaycasts = false;

        buttonTemplate.gameObject.SetActive(false);

        for (int i = 0; i < startingCount; i++)
        {
            CreateTargetSelectorButton();
        }
    }

    private static void CreateTargetSelectorButton()
    {
        TargetSelectorButton button = Instantiate(Instance.buttonTemplate, Instance.buttonTemplate.transform.parent);
        button.gameObject.SetActive(true);

        button.TargetSelector = Instance;
        button.Deactivate(0);

        _buttons.Add(button);
        _buttonsCount++;
    }

    private void Appear()
    {
        panel.DOKill();
        panel.DOFade(_panelAlpha, fadeTime);
        canvasGroup.blocksRaycasts = true;
    }

    private void Disappear()
    {
        panel.DOKill();
        panel.DOFade(0, fadeTime);
        canvasGroup.blocksRaycasts = false;
    }

    public static IEnumerator SelectTeam()
    {
        Instance.canvasGroup.blocksRaycasts = true;

        _chosen = false;
        TeamResult = null;

        Instance.teamSelectGroup.DOFade(1, Instance.fadeTime).onComplete = () =>
        {
            Instance.teamSelectGroup.blocksRaycasts = true;
        };

        Instance.teamSelectGroup.blocksRaycasts = false;

        yield return new WaitUntil(() => _chosen);

        Instance.teamSelectGroup.DOFade(0, Instance.fadeTime);
        Instance.teamSelectGroup.blocksRaycasts = false;

        Instance.canvasGroup.blocksRaycasts = false;
    }

    public static IEnumerator SelectTarget(Team team)
    {
        return SelectTarget(TargetManager.GetTargets(team));
    }

    public static IEnumerator SelectTarget(List<Target> targets)
    {
        return SelectTarget(targets, targets.Count);
    }

    public static IEnumerator SelectTarget(List<Target> targets, int count)
    {
        Instance.Appear();

        for (int i = 0; i < count; i++)
        {
            while (i >= _buttonsCount)
            {
                CreateTargetSelectorButton();
            }

            TargetSelectorButton button = _buttons[i];
            button.Activate(Instance.fadeTime);

            button.Target = targets[i];
        }

        _chosen = false;
        TargetResult = null;

        yield return new WaitUntil(() => _chosen);

        foreach (TargetSelectorButton button in _buttons)
        {
            button.Deactivate(Instance.fadeTime);
            button.Target = null;
        }

        Instance.Disappear();
    }

    public void ChooseTeam(Team team)
    {
        _chosen = true;
        TeamResult = team;
    }

    public void ChoosePlayerTeam()
    {
        ChooseTeam(Team.Player);
    }

    public void ChooseEnemyTeam()
    {
        ChooseTeam(Team.Enemy);
    }

    public void ChooseTarget(Target target)
    {
        _chosen = true;
        TargetResult = target;
    }
}
