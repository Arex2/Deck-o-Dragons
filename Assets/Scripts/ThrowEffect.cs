using UnityEngine;
using System.Collections;

public class ThrowEffect : MonoBehaviour
{
    public GameObject poofPrefab;
    public GameObject meatballPrefab;
    public Transform poofSpawnPoint;

    private Vector3 originalSpawnPosition;

    public Rigidbody2D rb;

    private CanvasGroup canvasGroup;

    void Start()
    {
        originalSpawnPosition = transform.position;
        canvasGroup = GetComponentInParent<CanvasGroup>();
    }

    public void SpawnPoof()
    {

        GameObject poof = Instantiate(poofPrefab, poofSpawnPoint.position, Quaternion.identity);


        ParticleSystem ps = poof.GetComponent<ParticleSystem>();

        if (ps != null)
        {

            Destroy(poof, ps.main.duration);
        }

        StartCoroutine(ResetMeatballAfterDelay(0.15f));
    }

    private IEnumerator ResetMeatballAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        ResetMeatball();
    }

    void ResetMeatball()
    {
        gameObject.SetActive(false);

        transform.position = originalSpawnPosition;
        transform.rotation = Quaternion.identity;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        gameObject.SetActive(true);
    }
}
