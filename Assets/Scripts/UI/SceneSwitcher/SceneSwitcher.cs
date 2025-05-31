using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityScene = UnityEngine.SceneManagement.Scene;
using DG.Tweening;

[SingletonMode(true)]
public class SceneSwitcher : Singleton<SceneSwitcher>
{
    public static int CurrentSceneBuildIndex => SceneManager.GetActiveScene().buildIndex;

    public static bool DoingTransition { get; private set; }

    public static Action<int> OnSwitchScene { get; set; }

    [CacheComponent] [SerializeField] private CanvasGroup canvasGroup;

    public Vector2 WholeScreenSize => screenOverlay.rectTransform.rect.size;

    public CanvasGroup CanvasGroup => canvasGroup;
    public Image ScreenOverlay => screenOverlay;

    [Header("Transition")]
    [SerializeField] private Image screenOverlay;

    [Space]
    [SerializeField] private float duration;
    [SerializeField] private Ease inEase;
    [SerializeField] private Ease outEase;

    [Space]
    [SerializeField] private SceneSwitcherTransition defaultTransition;

    public SceneSwitcherTransition CurrentTransition => _currentTransition != null ? _currentTransition : defaultTransition;
    private SceneSwitcherTransition _currentTransition;

    private Dictionary<int, SceneSwitcherTransition> _transitionDictionary = new();

    [Header("Scenes")]
    [SerializeField] private SceneReference mainMenuScene;
    [SerializeField] private SceneReference creditsScene;

    [Space]
    [SerializeField] private SceneReference gardenScene;
    [SerializeField] private SceneReference eggScene;
    [Space]
    [SerializeField] private SceneReference battleSelectionScene;
    [SerializeField] private SceneReference deckViewerScene;

    [Space]
    [SerializeField] private SceneReference cardGameScene;
    [SerializeField] private SceneReference cardShopScene;

    [Header("Sounds")]
    [SerializeField] private AudioClip transitionSoundClip;

    protected override void Awake()
    {
        base.Awake();

        ScreenOverlay.enabled = false;

        foreach (SceneSwitcherTransition transition in GetComponentsInChildren<SceneSwitcherTransition>(true))
        {
            transition.Initialize();

            if (transition == defaultTransition)
            {
                continue;
            }

            foreach (Scene scene in transition.Scenes)
            {
                _transitionDictionary[GetSceneInternal(scene).BuildIndex] = transition;
            }
        }

        SetCurrentTransition(-999, true);
        CurrentTransition.Appear(0);
        //CurrentTransition.RectTransform.localScale = Vector3.zero;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        IntroAnim();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityScene scene, LoadSceneMode loadSceneMode)
    {
        IntroAnim();
    }

    private void IntroAnim()
    {
        EventSystem.current.SetSelectedGameObject(null);

        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = false;

        DoingTransition = true;

        CurrentTransition.Disappear(duration, outEase, () =>
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;

            DoingTransition = false;
        });
    }

    public static void SwitchScene(int sceneIndex, Action onFinish = null)
    {
        Instance.StartCoroutine(Instance.SwitchSceneCoroutine(sceneIndex, onFinish));
    }

    private IEnumerator SwitchSceneCoroutine(int sceneIndex, Action onFinish = null)
    {
        if (DoingTransition)
        {
            yield break;
        }

        DoingTransition = true;

        OnSwitchScene?.Invoke(sceneIndex);

        SetCurrentTransition(sceneIndex);

        if (sceneIndex == eggScene.BuildIndex)
        {
            //viktig
            DragonActive.doCheck = true;
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(transitionSoundClip);
        }

        EventSystem.current.SetSelectedGameObject(null);

        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;

        bool tweenComplete = false;
        CurrentTransition.Appear(duration, inEase, () =>
        {
            tweenComplete = true;
        });

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);

        asyncOperation.allowSceneActivation = false;

        yield return new WaitUntil(() => asyncOperation.progress >= 0.9f);

        yield return new WaitUntil(() => tweenComplete);

        // Switch scene
        asyncOperation.allowSceneActivation = true;

        onFinish?.Invoke();
    }

    private void SetCurrentTransition(int buildIndex, bool isInit = false)
    {
        if (!isInit)
        {
            CurrentTransition.Disable();
        }

        if (buildIndex < 0)
        {
            _currentTransition = null;
        }
        else
        {
            _transitionDictionary.TryGetValue(buildIndex, out _currentTransition);
        }

        CurrentTransition.Enable();
    }

    public static void SwitchToMainMenu(Action onFinish = null) => SwitchScene(Instance.mainMenuScene, onFinish);
    public static void SwitchToCredits(Action onFinish = null) => SwitchScene(Instance.creditsScene, onFinish);

    public static void SwitchToGarden(Action onFinish = null) => SwitchScene(Instance.gardenScene, onFinish);
    public static void SwitchToEgg(Action onFinish = null) => SwitchScene(Instance.eggScene, onFinish);

    public static void SwitchToBattleSelection(Action onFinish = null) => SwitchScene(Instance.battleSelectionScene, onFinish);
    public static void SwitchToDeckViewer(Action onFinish = null) => SwitchScene(Instance.deckViewerScene, onFinish);

    public static void SwitchToCardGame(Action onFinish = null) => SwitchScene(Instance.cardGameScene, onFinish);
    public static void SwitchToCardShop(Action onFinish = null) => SwitchScene(Instance.cardShopScene, onFinish);

    public static SceneReference GetScene(Scene scene) => Instance.GetSceneInternal(scene);

    private SceneReference GetSceneInternal(Scene scene)
    {
        return scene switch
        {
            Scene.MainMenu => mainMenuScene,
            Scene.Credits => creditsScene,
            Scene.Garden => gardenScene,
            Scene.Egg => eggScene,
            Scene.BattleSelection => battleSelectionScene,
            Scene.DeckViewer => deckViewerScene,
            Scene.CardGame => cardGameScene,
            Scene.CardShop => cardShopScene,
            _ => null
        };
    }

    [Serializable]
    public enum Scene
    {
        MainMenu,
        Credits,
        Garden,
        Egg,
        BattleSelection,
        DeckViewer,
        CardGame,
        CardShop,
    }
}
