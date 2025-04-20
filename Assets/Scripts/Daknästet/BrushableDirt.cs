using UnityEngine;
using System.Collections;

public class BrushableDirt : MonoBehaviour
{
    public Sprite[] dirtLevel; 
    [SerializeField] private float[] stageTimes = new float[] { 1f, 2f, 3f };

    public AudioClip brushingSound;
    public AudioClip finishedSound;
    //public GameObject cleanParticles;

    private SpriteRenderer sr;
    private AudioSource audioSource;
    private float brushTimer = 0f;
    private bool isTouching = false;
    private bool finished = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;
    }

    void Update()
    {
        if (finished) return;

        if (BrushingController.instance.isBrushing && isTouching)
        {
            brushTimer += Time.deltaTime;

            if (!audioSource.isPlaying)
            {
                audioSource.clip = brushingSound;
                audioSource.Play();
            }

            if (brushTimer >= stageTimes[2])
            {
                FinishCleaning();
            }
            else if (brushTimer >= stageTimes[1])
            {
                sr.sprite = dirtLevel[2];
            }
            else if (brushTimer >= stageTimes[0])
            {
                sr.sprite = dirtLevel[1];
            }
            else
            {
                sr.sprite = dirtLevel[0];
            }
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
    }


    void OnMouseDown()
    {
        if (BrushingController.instance.isBrushing)
        {
            isTouching = true;
        }
    }

    void OnMouseUp()
    {
        isTouching = false;
    }

    void FinishCleaning()
    {
        if (finished) return;
        finished = true;

        audioSource.Stop();
        AudioSource.PlayClipAtPoint(finishedSound, transform.position);

     //   if (cleanParticles != null)
     //       Instantiate(cleanParticles, transform.position, Quaternion.identity);

        StartCoroutine(DisableAfterDelay(0.1f));
    }

    IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
