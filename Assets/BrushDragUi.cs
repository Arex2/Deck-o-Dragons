using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class BrushDragUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public GameObject targetObject;
    public AudioClip brushingSound;

    private AudioSource audioSource;
    private Animator brushAnimator;
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private bool isBrushing = false;
    private bool isDragging = false;

    private Vector3 initialPosition; // <-- Här lagrar vi startposition

    // Rörelselogik
    private List<(Vector2 position, float time)> movementHistory = new List<(Vector2, float)>();
    private float brushMovementThreshold = 10f;
    private float sampleWindow = 0.1f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        audioSource = GetComponent<AudioSource>();
        brushAnimator = GetComponent<Animator>();

        initialPosition = rectTransform.localPosition; // <-- Spara startläge
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = false;

        isDragging = true;
        movementHistory.Clear();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = Input.mousePosition;

        if (!isDragging || !IsOverTarget())
        {
            StopBrushing();
            return;
        }

        TrackMovement();

        float moved = CalculateMovementInWindow(sampleWindow);

        if (moved >= brushMovementThreshold)
        {
            StartBrushing();
        }
        else
        {
            StopBrushing();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = true;

        isDragging = false;
        StopBrushing();
        ResetPosition(); // <-- Återställ position när släppt
    }

    private void ResetPosition()
    {
        rectTransform.localPosition = initialPosition;
    }

    private bool IsOverTarget()
    {
        if (targetObject == null) return false;

        RectTransform targetRect = targetObject.GetComponent<RectTransform>();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(targetRect, Input.mousePosition, canvas.worldCamera, out localPoint);
        return targetRect.rect.Contains(localPoint);
    }

    private void StartBrushing()
    {
        if (isBrushing) return;

        isBrushing = true;

        if (brushAnimator != null)
            brushAnimator.SetBool("IsBrushing", true);

        if (audioSource != null && brushingSound != null && !audioSource.isPlaying)
        {
            audioSource.loop = true;
            audioSource.clip = brushingSound;
            audioSource.Play();
        }
    }

    private void StopBrushing()
    {
        if (!isBrushing) return;

        isBrushing = false;

        if (brushAnimator != null)
            brushAnimator.SetBool("IsBrushing", false);

        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();
    }

    private void TrackMovement()
    {
        Vector2 currentPos = rectTransform.anchoredPosition;
        float currentTime = Time.time;

        movementHistory.Add((currentPos, currentTime));
        movementHistory.RemoveAll(entry => currentTime - entry.time > sampleWindow);
    }

    private float CalculateMovementInWindow(float timeWindow)
    {
        float total = 0f;
        for (int i = 1; i < movementHistory.Count; i++)
        {
            total += Vector2.Distance(movementHistory[i - 1].position, movementHistory[i].position);
        }
        return total;
    }
}
