using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class DraggableUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public GameObject targetObject; 
    public AudioClip collisionSound; 
    private Animator meatballAnimator;  
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector3 initialPosition; 
    private CanvasGroup canvasGroup;

    [SerializeField] private AudioClip happySound;

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

            AudioManager.Instance.PlaySFX(collisionSound);

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


        GameObject hearts = FindInactiveHeartWithTag();
        if (hearts != null)
        {
            AudioManager.Instance.PlaySFX(happySound);
            StartCoroutine(ActivateHeartTemporarily(hearts));
        }
        else
        {
            Debug.LogWarning("Inactive Heart with tag 'Heart' not found in the scene.");
        }
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



    private void ResetPosition()
    {
        rectTransform.localPosition = initialPosition;
    }
}
