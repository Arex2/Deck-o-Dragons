using System;
using UnityEngine;

public class DragonController : MonoBehaviour
{
    DragonActive dragonActive;
    [SerializeField] private GameObject drakPrefab;
    [SerializeField] private Vector2 drakStart = Vector2.zero;

    private void Awake()
    {
        DontDestroyOnLoad(drakPrefab);
        dragonActive = GameObject.Find("DragonActive").GetComponent<DragonActive>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SpawnNewDragon();
        }
    }

    /*void OnMouseDown()
    {
        SpawnNewDragon();
    }*/

    /*public static void InitiateSpawn()
    {
        SpawnNewDragon();
    }*/

    public void SpawnNewDragon()
    {
        //if (drakPrefab != null)
        //{
        if(DragonActive.age == 1)
        {
            Instantiate(dragonActive.teenDragons[DragonActive.index], new Vector3(0, 0, 0), Quaternion.identity);
            DragonActive.age++;
            Debug.Log(DragonActive.age);
        }
        else if (DragonActive.age == 2)
        {
            Instantiate(dragonActive.adultDragons[DragonActive.index], new Vector3(0, 0, 0), Quaternion.identity);
            DragonActive.age++;
            Debug.Log(DragonActive.age);
        }
        else if (DragonActive.age == 3)
        {
            Instantiate(dragonActive.egg, new Vector3(0, 0, 0), Quaternion.identity);
            DragonActive.age = 0;
            Debug.Log(DragonActive.age);
        }
        else
        {
            return;
        }

        /*DragonActive.drPref = drakPrefab;
        Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
        Instantiate(drakPrefab, drakStart, Quaternion.identity);*/
        Destroy(gameObject);
        //}
    }
}
