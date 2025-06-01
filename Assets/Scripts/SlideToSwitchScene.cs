using UnityEngine;
using UnityEngine.EventSystems;

public class SlideToSwitchScene : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private SceneSwitcher.Scene sceneToLoad;
    [SerializeField] private float triggerDistancePercent = 0.9f;
    [SerializeField] private AudioClip dragSound;
    [SerializeField] private float minDragDistance = 30f;

    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 initialPosition;
    private CanvasGroup canvasGroup;
    private bool hasSwitched = false;
    private bool dragSoundPlayed = false;

    private Vector2 dragOffset;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        initialPosition = rectTransform.anchoredPosition;

        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = false;

        hasSwitched = false;
        dragSoundPlayed = false;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out Vector2 localPoint
        );

        dragOffset = rectTransform.anchoredPosition - localPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (hasSwitched) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out Vector2 localPoint
        );

        float newX = localPoint.x + dragOffset.x;
        rectTransform.anchoredPosition = new Vector2(newX, initialPosition.y);

        float screenWidth = Screen.width;
        float currentX = rectTransform.position.x;
        float dragPercent = currentX / screenWidth;

        float dragDistance = Mathf.Abs(rectTransform.anchoredPosition.x - initialPosition.x);

        if (dragPercent >= triggerDistancePercent && dragDistance >= minDragDistance)
        {
            hasSwitched = true;
            SwitchScene();
        }

        // Spela ljud vid första drag
        if (!dragSoundPlayed && dragSound != null)
        {
            AudioManager.Instance.PlaySFX(dragSound);
            dragSoundPlayed = true;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = true;

        if (!hasSwitched)
        {
            rectTransform.anchoredPosition = initialPosition;
        }
    }

    private void SwitchScene()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayClickSound();
        }

        SceneSwitcher.SwitchScene(SceneSwitcher.GetScene(sceneToLoad));
    }
}
