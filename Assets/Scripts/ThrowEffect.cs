using UnityEngine;

public class ThrowEffect : MonoBehaviour
{
    public GameObject poofPrefab;

    public void SpawnPoof()
    {
        //Instantiate(poofPrefab, transform.position, Quaternion.identity, transform.parent);
        Destroy(gameObject);

    }
}
