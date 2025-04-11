using UnityEngine;

public class DragonController : MonoBehaviour
{
    [SerializeField] private GameObject drakPrefab;
    [SerializeField] private Vector2 drakStart = Vector2.zero;

    void OnMouseDown()
    {
        SpawnNewDragon();
    }

    private void SpawnNewDragon()
    {
        if (drakPrefab != null)
        {
            Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
            Instantiate(drakPrefab, drakStart, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
