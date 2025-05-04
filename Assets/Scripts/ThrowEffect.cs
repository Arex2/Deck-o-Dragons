using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class DraggableUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public GameObject targetObject; 
    public AudioClip collisionSound; 
    private AudioSource audioSource; 
    private Animator meatballAnimator;  
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector3 initialPosition; 
    private CanvasGroup canvasGroup; 

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        initialPosition = rectTransform.localPosition;
        audioSource = GetComponent<AudioSource>();
        meatballAnimator = GetComponent<Animator>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = true;

        if (targetObject != null && IsCollidingWithTarget())
        {
            Debug.Log("Triggering CollisionAnimation on Meatball");

            if (meatballAnimator != null)
                meatballAnimator.SetTrigger("CollisionAnimation");

            if (audioSource != null && collisionSound != null)
                audioSource.PlayOneShot(collisionSound);

            StartCoroutine(ResetAfterAnimation());
        }
        else
        {
            ResetPosition();
        }
    }

    private bool IsCollidingWithTarget()
    {
        RectTransform targetRect = targetObject.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(
            targetRect,
            Input.mousePosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera
        );
    }

    private IEnumerator ResetAfterAnimation()
    {
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

        ResetPosition();
    }

    private void ResetPosition()
    {
        rectTransform.localPosition = initialPosition;
    }
}
