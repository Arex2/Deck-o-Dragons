using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class BrushDragUI : MonoBehaviour//, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public GameObject targetObject;

    private Animator brushAnimator;
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private bool isBrushing = false;
    private bool isDragging = false;
    private bool isStillOnTarget = false;

    private Vector3 initialPosition;

    private List<(Vector2 position, float time)> movementHistory = new List<(Vector2, float)>();
    private float brushMovementThreshold = 10f;
    private float sampleWindow = 0.1f;

    private GameObject[] dirtObjects;
    private bool[] dirtRemoved;
    private float[] dirtTimers;

    private float brushingTime = 0f;

    private bool glitterPlayed = false;

    [SerializeField] private AudioClip brushingSound, shinySound;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        brushAnimator = GetComponent<Animator>();

        initialPosition = rectTransform.localPosition;
    }

    public void OnBeginDrag()//PointerEventData eventData)
    {
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = false;

        isDragging = true;
        movementHistory.Clear();

        rectTransform.DOKill();
    }

    public void OnDrag(Vector2 mousePos)//PointerEventData eventData)
    {
        rectTransform.position = mousePos;

        if (IsOverTarget())
        {
            isStillOnTarget = true;
        }
        else
        {
            isStillOnTarget = false;
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

    public void OnEndDrag()//PointerEventData eventData)
    {
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = true;

        isDragging = false;

        ResetPosition();
        StopBrushing();
    }

    private void ResetPosition()
    {
        rectTransform.DOKill();
        rectTransform.DOLocalMove(initialPosition, 0.5f).SetEase(Ease.OutExpo);

        if (!isDragging)
        {
            if (isBrushing)
            {
                for (int i = 0; i < dirtTimers.Length; i++)
                {
                    dirtTimers[i] = 0f;
                }

                brushingTime = 0f;
            }
        }
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

        RefreshDirt();

        if (brushAnimator != null)
            brushAnimator.SetBool("IsBrushing", true);


    }

    private void StopBrushing()
    {
        if (!isBrushing) return;

        isBrushing = false;

        if (brushAnimator != null)
            brushAnimator.SetBool("IsBrushing", false);

        if (dirtObjects != null && dirtObjects.Length > 0)
        {
            foreach (GameObject dirt in dirtObjects)
            {
                if (dirt != null && !dirtRemoved[System.Array.IndexOf(dirtObjects, dirt)])
                {
                    dirt.SetActive(true);
                }
            }
        }

        if (dirtRemoved != null)
        {
            for (int i = 0; i < dirtRemoved.Length; i++)
            {
                dirtRemoved[i] = false;
            }
        }
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

    private void Update()
    {
        if (isBrushing)
        {
            GameObject[] currentDirt = GameObject.FindGameObjectsWithTag("Dirt");
            if (dirtObjects == null || currentDirt.Length != dirtObjects.Length)
            {
                RefreshDirt();
            }

            if (!isStillOnTarget)
            {
                brushingTime += Time.deltaTime;
            }

            for (int i = 0; i < dirtObjects.Length; i++)
            {
                if (dirtObjects[i] == null) continue;

                dirtTimers[i] += Time.deltaTime;
                if (dirtTimers[i] >= (i + 1) && !dirtRemoved[i])
                {
                    dirtObjects[i].SetActive(false);
                    dirtRemoved[i] = true;
                }
            }

            if (!glitterPlayed && AllDirtRemoved())
            {
                glitterPlayed = true;
                StartCoroutine(PlayGlitterEffect());
            }
        }
    }

    private bool AllDirtRemoved()
    {
        foreach (bool removed in dirtRemoved)
        {
            if (!removed)
                return false;
        }
        return true;
    }

    private void RefreshDirt()
    {
        dirtObjects = GameObject.FindGameObjectsWithTag("Dirt");
        dirtRemoved = new bool[dirtObjects.Length];
        dirtTimers = new float[dirtObjects.Length];

        for (int i = 0; i < dirtObjects.Length; i++)
        {
            dirtRemoved[i] = false;
            dirtTimers[i] = 0f;
            if (dirtObjects[i] != null)
                dirtObjects[i].SetActive(true);
        }

        glitterPlayed = false;
    }

    private IEnumerator PlayGlitterEffect()
    {
        AudioManager.Instance.PlaySFX(shinySound);
        List<GameObject> glittersToToggle = new List<GameObject>();
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("Glitter") && !obj.activeInHierarchy && obj.scene.IsValid())
            {
                obj.SetActive(true);
                glittersToToggle.Add(obj);
            }
        }

        yield return new WaitForSeconds(0.4f);

        foreach (GameObject glitter in glittersToToggle)
        {
            if (glitter != null)
                glitter.SetActive(false);
        }
    }


    public void PlayBrushingSound()
    {
        AudioManager.Instance.PlaySFX(brushingSound);
    }
}
