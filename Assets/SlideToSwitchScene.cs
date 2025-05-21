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
    private Vector3 initialPosition;
    private CanvasGroup canvasGroup;
    private bool hasSwitched = false;

    private Vector2 dragOffset;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        initialPosition = rectTransform.localPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = false;

        hasSwitched = false;


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

        rectTransform.anchoredPosition = localPoint + dragOffset;


        float screenWidth = Screen.width;
        float currentX = rectTransform.position.x;
        float dragPercent = currentX / screenWidth;


        float dragDistance = Vector3.Distance(rectTransform.localPosition, initialPosition);

        if (dragPercent >= triggerDistancePercent && dragDistance >= minDragDistance)
        {
            hasSwitched = true;
            SwitchScene();
        }
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = true;

        if (!hasSwitched)
        {
            rectTransform.localPosition = initialPosition;
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
