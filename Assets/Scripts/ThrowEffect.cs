using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public GameObject targetObject;  // The target UI element to check collision with
    public Animator targetAnimator;  // Animator for the target object (to play animation)
    public AudioClip collisionSound; // Audio clip to play on collision
    private AudioSource audioSource; // AudioSource for playing sound
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
        initialPosition = rectTransform.localPosition;  // Store position relative to the canvas
        audioSource = GetComponent<AudioSource>();  // Get the AudioSource component on the draggable object
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Optional: Disable raycasts during drag for smoother movement
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move the object while dragging
        Vector2 position = Input.mousePosition;
        rectTransform.position = position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Re-enable raycasts after drag is done
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
        }

        // Check for collision after dragging ends
        if (targetObject != null && IsCollidingWithTarget())
        {
            // Play the animation on collision
            if (targetAnimator != null)
            {
                targetAnimator.SetTrigger("CollisionAnimation"); // Replace with the correct trigger name
            }

            // Play the collision sound
            if (audioSource != null && collisionSound != null)
            {
                audioSource.PlayOneShot(collisionSound);
            }

            // Immediately reset the draggable object to its original position relative to the canvas
            ResetPosition();
        }
    }

    private bool IsCollidingWithTarget()
    {
        // Check if the draggable object collides with the target object
        RectTransform targetRect = targetObject.GetComponent<RectTransform>();

        // Check if the draggable object is within the target's bounds
        return targetRect.rect.Contains(rectTransform.localPosition);
    }

    private void ResetPosition()
    {
        // Reset draggable object position to its original position in local space relative to the canvas
        rectTransform.localPosition = initialPosition;
    }
}
