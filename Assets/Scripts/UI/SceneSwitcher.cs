using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using DG.Tweening;

[SingletonMode(true)]
public class SceneSwitcher : Singleton<SceneSwitcher>
{
    public static int CurrentSceneBuildIndex => SceneManager.GetActiveScene().buildIndex;

    [CacheComponent] [SerializeField] private CanvasGroup canvasGroup;

    [Header("Transition")]
    [SerializeField] private RectTransform wholeScreenRect;
    [SerializeField] private Image transitionImage;
    private RectTransform _transitionImageRect;
    [SerializeField] private float safeArea;

    private float _bigScale;

    [Space]
    [SerializeField] private float duration;
    [SerializeField] private Ease inEase;
    [SerializeField] private Ease outEase;

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

    private Vector2Int _oldScreenSize;

    protected override void Awake()
    {
        base.Awake();

        if (transitionImage == null || wholeScreenRect == null)
        {
            return;
        }

        _transitionImageRect = transitionImage.transform as RectTransform;
        OnResolutionChanged();

        SceneManager.sceneLoaded += OnSceneLoaded;

        _transitionImageRect.localScale = Vector3.zero;
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

    private void Update()
    {
        if (_oldScreenSize.x == Screen.width && _oldScreenSize.y == Screen.height)
        {
            return;
        }

        OnResolutionChanged();
    }

    private void OnResolutionChanged()
    {
        _oldScreenSize.x = Screen.width;
        _oldScreenSize.y = Screen.height;

        Sprite transitionSprite = transitionImage.sprite;

        Rect rect = wholeScreenRect.rect;

        float spriteWidth = transitionSprite.texture.width;
        float spriteHeight = transitionSprite.texture.height;

        Vector2 size = rect.size;

        float spriteRatio = spriteWidth / spriteHeight;
        float rectRatio = size.x / size.y;

        float scale;

        // Too Tall
        if (spriteRatio > rectRatio)
        {
            float oldHeight = size.y;

            size.y = size.x * (1f / spriteRatio);

            scale = oldHeight / size.y;
        }
        // Too Wide
        else
        {
            float oldWidth = size.x;

            size.x = size.y * spriteRatio;

            scale = oldWidth / size.x;
        }

        _bigScale = scale + safeArea;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode loadSceneMode)
    {
        IntroAnim();
    }

    private void IntroAnim()
    {
        EventSystem.current.SetSelectedGameObject(null);

        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = false;

        _transitionImageRect.DOKill();

        _transitionImageRect.DOScale(_bigScale, duration).SetEase(inEase).onComplete = () =>
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
        };
    }

    public static void SwitchScene(int sceneIndex, Action onFinish = null)
    {
        Instance.StartCoroutine(Instance.SwitchSceneCoroutine(sceneIndex, onFinish));
    }

    private IEnumerator SwitchSceneCoroutine(int sceneIndex, Action onFinish = null)
    {
        if (sceneIndex == eggScene.BuildIndex)
        {
            //viktig
            DragonActive.doCheck = true;
        }

        EventSystem.current.SetSelectedGameObject(null);

        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;

        _transitionImageRect.DOKill();

        bool tweenComplete = false;
        _transitionImageRect.DOScale(Vector2.zero, duration).SetEase(outEase).onComplete = () =>
        {
            tweenComplete = true;
        };

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);

        asyncOperation.allowSceneActivation = false;

        yield return new WaitUntil(() => asyncOperation.progress >= 0.9f);

        yield return new WaitUntil(() => tweenComplete);

        // Switch scene
        asyncOperation.allowSceneActivation = true;

        onFinish?.Invoke();
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

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            return;
        }

        DrawGizmos();
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        DrawGizmos();
    }

    private void DrawGizmos()
    {
        if (transitionImage == null)
        {
            return;
        }

        if (wholeScreenRect == null)
        {
            return;
        }

        Sprite transitionSprite = transitionImage.sprite;

        if (transitionSprite == null)
        {
            return;
        }

        Rect rect = wholeScreenRect.rect;

        float spriteWidth = transitionSprite.texture.width;
        float spriteHeight = transitionSprite.texture.height;

        Vector2 size = rect.size;

        float spriteRatio = spriteWidth / spriteHeight;
        float rectRatio = size.x / size.y;
        float scale;

        // Too Tall
        if (spriteRatio > rectRatio)
        {
            float oldHeight = size.y;

            size.y = size.x * (1f / spriteRatio);

            scale = oldHeight / size.y;
        }
        // Too Wide
        else
        {
            float oldWidth = size.x;

            size.x = size.y * spriteRatio;

            scale = oldWidth / size.x;
        }

        Matrix4x4 startMatrix = Gizmos.matrix;

        Gizmos.matrix = wholeScreenRect.localToWorldMatrix;

        // Draw regular cube
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(Vector3.zero, size);

        // Draw safe area cube
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, size - (size * safeArea));

        // Draw scaled cube
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, size * scale);

        // Draw scaled cube with safe area
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(Vector3.zero, size * (scale + safeArea));

        Gizmos.matrix = startMatrix;
    }
#endif
}
