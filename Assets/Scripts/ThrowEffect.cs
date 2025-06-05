using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using DG.Tweening;

public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject targetObject;
    public AudioClip collisionSound;
    private Animator meatballAnimator;
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector3 initialPosition;
    private CanvasGroup canvasGroup;

    [SerializeField] private AudioClip happySound;

    private Vector2 pointerOffset;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        initialPosition = rectTransform.localPosition;
        meatballAnimator = GetComponent<Animator>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = false;

        RectTransform parentRect = rectTransform.parent as RectTransform;

        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            eventData.position,
            canvas.worldCamera,
            out localPointerPosition))
        {
            pointerOffset = (Vector2)rectTransform.localPosition - localPointerPosition;
        }

        rectTransform.DOKill();
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransform parentRect = rectTransform.parent as RectTransform;

        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            eventData.position,
            canvas.worldCamera,
            out localPointerPosition))
        {
            rectTransform.localPosition = localPointerPosition + pointerOffset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = true;

        rectTransform.DOKill();

        if (targetObject != null && IsCollidingWithTarget(eventData.position))
        {
            Debug.Log("Triggering CollisionAnimation on Meatball");

            if (meatballAnimator != null)
                meatballAnimator.SetTrigger("CollisionAnimation");

            AudioManager.Instance.PlaySFX(collisionSound);

            StartCoroutine(ResetAfterAnimation());
        }
        else
        {
            ResetPosition(false);
        }
    }

    private bool IsCollidingWithTarget(Vector2 screenPoint)
    {
        if (targetObject == null)
            return false;

        RectTransform targetRect = targetObject.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(
            targetRect,
            screenPoint,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera
        );
    }

    private IEnumerator ResetAfterAnimation()
    {
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = false;

        if (meatballAnimator != null)
        {
            yield return null;
            AnimatorStateInfo stateInfo = meatballAnimator.GetCurrentAnimatorStateInfo(0);
            float timeout = 0.5f;
            float elapsed = 0f;
            while (!stateInfo.IsName("CollisionAnimation") && elapsed < timeout)
            {
                yield return null;
                stateInfo = meatballAnimator.GetCurrentAnimatorStateInfo(0);
                elapsed += Time.deltaTime;
            }

            while (stateInfo.IsName("CollisionAnimation") && stateInfo.normalizedTime < 1f)
            {
                yield return null;
                stateInfo = meatballAnimator.GetCurrentAnimatorStateInfo(0);
            }
        }

        GameObject hearts = FindInactiveHeartWithTag();
        if (hearts != null)
        {
            AudioManager.Instance.PlaySFX(happySound);
            StartCoroutine(ActivateHeartTemporarily(hearts));
        }
        else
        {
            Debug.LogWarning("Inactive Heart with tag 'Hearts' not found in the scene.");
        }

        yield return new WaitForSeconds(0.3f);

        ResetPosition(true);

        yield return new WaitForSeconds(0.1f);

        if (meatballAnimator != null)
            meatballAnimator.SetTrigger("Done");

        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = true;
    }

    private IEnumerator ActivateHeartTemporarily(GameObject heart)
    {
        heart.SetActive(true);
        yield return new WaitForSeconds(0.7f);
        heart.SetActive(false);
    }

    private GameObject FindInactiveHeartWithTag()
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("Hearts") && !obj.activeInHierarchy && obj.scene.IsValid())
            {
                return obj;
            }
        }
        return null;
    }

    private void ResetPosition(bool instant)
    {
        rectTransform.DOKill();

        if (instant)
        {
            rectTransform.localPosition = initialPosition;
        }
        else
        {
            rectTransform.DOLocalMove(initialPosition, 0.5f).SetEase(Ease.OutExpo);
        }
    }
}
