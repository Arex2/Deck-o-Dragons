using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    //From this tutorial: https://www.youtube.com/watch?app=desktop&v=_ICCSDmLCX4&t=13s

    public float DestroyTime = 3f;
    public Vector3 Offset = new Vector3 (0, 0.5f, 0);
    public Vector3 RandomizeIntensity = new Vector3(0.5f, 0.1f,0);

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, DestroyTime);
        transform.localPosition += Offset;
        transform.localPosition += new Vector3(Random.Range(-RandomizeIntensity.x, RandomizeIntensity.x), Random.Range(-RandomizeIntensity.y, RandomizeIntensity.y), 0);
    }
}
