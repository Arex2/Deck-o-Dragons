using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityScene = UnityEngine.SceneManagement.Scene;
using DG.Tweening;

[SingletonMode(true)]
public class SceneSwitcher : Singleton<SceneSwitcher>
{
    public static int CurrentSceneBuildIndex => SceneManager.GetActiveScene().buildIndex;

    public static bool SwitchingScene { get; private set; }

    [CacheComponent] [SerializeField] private CanvasGroup canvasGroup;

    public Vector2 WholeScreenSize => wholeScreenRect.rect.size;

    [Header("Transition")]
    [SerializeField] private RectTransform wholeScreenRect;

    [Space]
    [SerializeField] private float duration;
    [SerializeField] private Ease inEase;
    [SerializeField] private Ease outEase;

    [Space]
    [SerializeField] private SceneSwitcherCutout defaultCutout;

    public SceneSwitcherCutout CurrentCutout => _currentCutout != null ? _currentCutout : defaultCutout;
    private SceneSwitcherCutout _currentCutout;

    private Dictionary<int, SceneSwitcherCutout> _cutoutDictionary = new();

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

        foreach (SceneSwitcherCutout cutout in GetComponentsInChildren<SceneSwitcherCutout>(true))
        {
            cutout.ImageEnabled = false;
            cutout.CalculateCutoutScale();
            cutout.SetToScale();

            if (cutout == defaultCutout)
            {
                continue;
            }

            foreach (Scene scene in cutout.Scenes)
            {
                _cutoutDictionary[GetSceneInternal(scene).BuildIndex] = cutout;
            }
        }

        SetCurrentCutout(CurrentSceneBuildIndex);
        CurrentCutout.RectTransform.localScale = Vector3.zero;

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

        CurrentCutout.DoTransitionOut(duration, outEase, () =>
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
        });
    }

    public static void SwitchScene(int sceneIndex, Action onFinish = null)
    {
        Instance.StartCoroutine(Instance.SwitchSceneCoroutine(sceneIndex, onFinish));
    }

    private IEnumerator SwitchSceneCoroutine(int sceneIndex, Action onFinish = null)
    {
        if (SwitchingScene)
        {
            yield break;
        }

        SwitchingScene = true;

        SetCurrentCutout(sceneIndex);

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
        CurrentCutout.DoTransitionIn(duration, inEase, () =>
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

        SwitchingScene = false;
    }

    private void SetCurrentCutout(int buildIndex)
    {
        CurrentCutout.ImageEnabled = false;

        if (buildIndex < 0)
        {
            _currentCutout = null;
        }
        else
        {
            _cutoutDictionary.TryGetValue(buildIndex, out _currentCutout);
        }

        CurrentCutout.ImageEnabled = true;
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
