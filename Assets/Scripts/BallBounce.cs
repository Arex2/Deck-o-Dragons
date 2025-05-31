using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class BallBouncer : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject ballPrefab;         
    private GameObject ballInstance;

    private Transform activeCharacter;
    private Transform ballAnchor;
    private Vector3 bounceRootBaseLocalPos;

    public float ballBounceHeight = 0.5f;
    public float characterBounceHeight = 0.2f;
    public float bounceSpeed = 5f;
    public float bounceDelay = 0.2f;

    private bool isBouncing = false;
    private float timer = 0f;
    private int bounceCount = 0;
    private float ballLifetime = 6f;

    public RectTransform draggableBallUI;
    public RectTransform dropTargetArea;
    private Vector3 initialBallUIPosition;
    private Canvas canvas;

    public float bounceDuration = 4.5f;


    public AudioClip[] bounceSounds;
    private int currentSoundIndex = 0;


    public float soundLeadTime = 0.5f;
    private float bouncePeriod;
    private bool soundPlayedThisBounce = false;

    void Start()
    {
        StartCoroutine(WaitForCharacterInstantiation());

        if (draggableBallUI != null)
        {
            initialBallUIPosition = draggableBallUI.localPosition;
            canvas = draggableBallUI.GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("BallBouncer: Canvas not found in parents of draggableBallUI.");
            }
        }
        else
        {
            Debug.LogError("BallBouncer: Assign draggableBallUI in inspector.");
        }
    }

    void Update()
    {
        if (activeCharacter == null)
        {
            DetectActiveCharacter();
        }

        if (isBouncing && ballInstance != null && ballAnchor != null && activeCharacter != null)
        {
            timer += Time.deltaTime;

            bouncePeriod = 2 * Mathf.PI / bounceSpeed;

            float cycleTime = timer % bouncePeriod;


            float lowestPointTime = (3 * Mathf.PI / 2) / bounceSpeed;


            float soundPlayTime = lowestPointTime - soundLeadTime;
            if (soundPlayTime < 0)
                soundPlayTime += bouncePeriod; 


            if (!soundPlayedThisBounce && cycleTime >= soundPlayTime)
            {
                PlayBounceSound();
                soundPlayedThisBounce = true;
            }
            else if (cycleTime < soundPlayTime)
            {

                soundPlayedThisBounce = false;
            }


            float ballY = ballAnchor.position.y + Mathf.Sin(timer * bounceSpeed) * ballBounceHeight;
            ballInstance.transform.position = new Vector3(ballAnchor.position.x, ballY, ballAnchor.position.z);


            float charY = Mathf.Sin(timer * bounceSpeed + bounceDelay) * characterBounceHeight;
            float targetY = bounceRootBaseLocalPos.y + charY;
            targetY = Mathf.Clamp(targetY, bounceRootBaseLocalPos.y - characterBounceHeight, bounceRootBaseLocalPos.y + characterBounceHeight);

            activeCharacter.localPosition = new Vector3(activeCharacter.localPosition.x, targetY, activeCharacter.localPosition.z);

            if (timer >= bounceDuration)
            {
                StopBouncing();
                ResetCharacterToNormal();
            }

            if (timer >= ballLifetime)
            {
                DestroyBallInstance();
            }
        }
    }

    private void PlayBounceSound()
    {
        if (bounceSounds != null && bounceSounds.Length > 0)
        {
            AudioManager.Instance.PlaySFX(bounceSounds[currentSoundIndex]);
            currentSoundIndex = (currentSoundIndex + 1) % bounceSounds.Length;  
        }
    }

    private IEnumerator WaitForCharacterInstantiation()
    {
        while (activeCharacter == null)
        {
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void DetectActiveCharacter()
    {
        if (activeCharacter != null)
            return;

        GameObject characterGO = FindActiveCharacter();
        if (characterGO == null)
        {
            return;
        }

        activeCharacter = characterGO.transform;

        ballAnchor = activeCharacter.Find("Anchor");
        if (ballAnchor == null)
        {
            return;
        }

        bounceRootBaseLocalPos = activeCharacter.localPosition;

        ballInstance = Instantiate(ballPrefab, ballAnchor.position, Quaternion.identity);
        ballInstance.SetActive(false);

        Animator animator = activeCharacter.GetComponent<Animator>();
        if (animator != null)
        {
            animator.applyRootMotion = false;
        }
    }

    private GameObject FindActiveCharacter()
    {
        string[] characterTags = { "Child", "Teen", "Adult" };

        foreach (string tag in characterTags)
        {
            GameObject found = GameObject.FindGameObjectWithTag(tag);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    public void StartBouncing()
    {
        if (ballInstance == null || activeCharacter == null || ballAnchor == null)
        {
            return;
        }

        ballInstance.SetActive(true);
        isBouncing = true;
        timer = 0f;
        bounceCount = 0;
        soundPlayedThisBounce = false;
        currentSoundIndex = 0; 
    }

    public void StopBouncing()
    {
        if (ballInstance != null)
        {
            ballInstance.SetActive(false);
            ballInstance.transform.position = ballAnchor.position;
        }
        if (activeCharacter != null)
        {
            activeCharacter.localPosition = bounceRootBaseLocalPos;
        }

        isBouncing = false;
    }

    private void ResetCharacterToNormal()
    {
        if (activeCharacter != null)
        {
            activeCharacter.localPosition = bounceRootBaseLocalPos;
        }
    }

    private void DestroyBallInstance()
    {
        if (ballInstance != null)
        {
            ballInstance.SetActive(false);
            ballInstance.transform.position = ballAnchor.position;
        }
    }

    private Vector2 pointerOffset;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (draggableBallUI == null || canvas == null)
            return;

        RectTransform parentRect = draggableBallUI.parent as RectTransform;

        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            eventData.position,
            canvas.worldCamera,
            out localPointerPosition))
        {
            pointerOffset = (Vector2)draggableBallUI.localPosition - localPointerPosition;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggableBallUI == null || canvas == null)
            return;

        RectTransform parentRect = draggableBallUI.parent as RectTransform;

        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            eventData.position,
            canvas.worldCamera,
            out localPointerPosition))
        {
            draggableBallUI.localPosition = localPointerPosition + pointerOffset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggableBallUI == null || dropTargetArea == null || canvas == null)
            return;

        Vector2 pointerPos = eventData.position;

        if (RectTransformUtility.RectangleContainsScreenPoint(dropTargetArea, pointerPos, canvas.worldCamera))
        {
            StartBouncing();

            draggableBallUI.localPosition = initialBallUIPosition;
        }
        else
        {
            draggableBallUI.localPosition = initialBallUIPosition;
        }
    }
}
