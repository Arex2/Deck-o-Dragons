using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;  // We need to include this for coroutines

public class DraggableUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public GameObject targetObject;  // The target UI element to check collision with
    public AudioClip collisionSound; // Audio clip to play on collision
    private AudioSource audioSource; // AudioSource for playing sound
    private Animator meatballAnimator;  // Animator for the meatball (to play animation)
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector3 initialPosition;  // Store the initial position of the draggable object in local canvas space
    private CanvasGroup canvasGroup;  // Optional: Can be used to manage raycasting or opacity during drag

    

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        // Store the initial local position relative to the canvas
        initialPosition = rectTransform.localPosition;
        audioSource = GetComponent<AudioSource>();
        meatballAnimator = GetComponent<Animator>();
    }
    


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
        }

        if (targetObject != null && IsCollidingWithTarget())
        {
            Debug.Log("Triggering CollisionAnimation on Meatball");

            if (meatballAnimator != null)
            {
                meatballAnimator.SetTrigger("CollisionAnimation");
            }

            if (audioSource != null && collisionSound != null)
            {
                audioSource.PlayOneShot(collisionSound);
            }

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
        return targetRect.rect.Contains(rectTransform.localPosition);
    }

    private IEnumerator ResetAfterAnimation()
    {
        if (meatballAnimator != null)
        {
            yield return null; // Wait 1 frame for trigger to register

            // Wait until the animation starts
            AnimatorStateInfo stateInfo = meatballAnimator.GetCurrentAnimatorStateInfo(0);
            float timeout = 0.5f; // To prevent infinite loops
            float elapsed = 0f;
            while (!stateInfo.IsName("CollisionAnimation") && elapsed < timeout)
            {
                yield return null;
                stateInfo = meatballAnimator.GetCurrentAnimatorStateInfo(0);
                elapsed += Time.deltaTime;
            }

            // Wait until animation is done
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
