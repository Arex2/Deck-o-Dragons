using UnityEngine;
using UnityEngine.UI;  // Import this for UI Button functionality
using System.Collections;

public class BallBouncer : MonoBehaviour
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

    private float ballLifetime = 6f;  // Increase the lifetime to 6 seconds for a longer bounce

    public Button bounceButton;  // Reference to the UI Button

    void Start()
    {
        StartCoroutine(WaitForCharacterInstantiation());

        // Set up the button click event
        if (bounceButton != null)
        {
            bounceButton.onClick.AddListener(StartBouncing);  // When button is clicked, it will start bouncing
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

            // Update ball position
            float ballY = ballAnchor.position.y + Mathf.Sin(timer * bounceSpeed) * ballBounceHeight;
            ballInstance.transform.position = new Vector3(ballAnchor.position.x, ballY, ballAnchor.position.z);

            // Update character position
            float charY = Mathf.Sin(timer * bounceSpeed + bounceDelay) * characterBounceHeight;
            float targetY = bounceRootBaseLocalPos.y + charY;
            targetY = Mathf.Clamp(targetY, bounceRootBaseLocalPos.y - characterBounceHeight, bounceRootBaseLocalPos.y + characterBounceHeight);

            activeCharacter.localPosition = new Vector3(activeCharacter.localPosition.x, targetY, activeCharacter.localPosition.z);

            // Debugging log for character position
            Debug.Log($"[Debug] Character Position: {activeCharacter.localPosition}");

            // Debugging log for Animator state and root motion
            Animator animator = activeCharacter.GetComponent<Animator>();
            if (animator != null)
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                Debug.Log($"Current Animator State: {stateInfo.IsName("Idle")}");
                Debug.Log($"Animator Apply Root Motion: {animator.applyRootMotion}");
            }

            // Debugging log for bouncing cycle
            if (Mathf.Abs(Mathf.Sin(timer * bounceSpeed)) < 0.01f)
            {
                bounceCount++;

                if (bounceCount >= 3)
                {
                    StopBouncing();
                    ResetCharacterToNormal();
                }
            }

            // Destroy ball after 6 seconds (or the desired time)
            if (timer >= ballLifetime)
            {
                DestroyBallInstance();
            }
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

        // Disable root motion on the Animator to prevent it from overriding position changes
        Animator animator = activeCharacter.GetComponent<Animator>();
        if (animator != null)
        {
            animator.applyRootMotion = false;  // Force it off
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

    // Function to destroy the ball after 6 seconds (adjust ball lifetime)
    private void DestroyBallInstance()
    {
        if (ballInstance != null)
        {
            Destroy(ballInstance);
            ballInstance = null;  // Set ballInstance to null to prevent future references to the destroyed object
        }
    }
}
