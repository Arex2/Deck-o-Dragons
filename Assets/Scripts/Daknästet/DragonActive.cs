using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DragonActive : MonoBehaviour
{
    private static DragonActive DrActInstance;

    [SerializeField] public GameObject egg;
    [SerializeField] public GameObject[] babyDragons;
    [SerializeField] public GameObject[] teenDragons;
    [SerializeField] public GameObject[] adultDragons;
    //public static GameObject drPref;
    public static bool dragonActive;
    public static int index;
    public static int age;

    void Awake()
    {
        if (DrActInstance == null)
        {
            DrActInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        /*if (egg == null)
        {
            egg = egg;
        }
        else
        {
            Destroy(gameObject);
        }

        if (DrActInstance == null)
        {
            DrActInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }*/

        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (!DragonActive.dragonActive)
            {
                Instantiate(egg, new Vector3(0, 0, 0), Quaternion.identity);
                //Egg.SpawnEgg(egg);
            }
        }
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (DragonActive.dragonActive)
            {
                SpawnDragons();
                Debug.Log(age);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (dragonActive == false)
        {
            DrActInstance = null;
        }
    }

    public void SpawnDragons()
    {
        if (DragonActive.age == 1)
        {
            Instantiate(babyDragons[index], new Vector3(0, 0, 0), Quaternion.identity);
        }
        else if (DragonActive.age == 2)
        {
            Instantiate(teenDragons[index], new Vector3(0, 0, 0), Quaternion.identity);
        }
        else if (DragonActive.age == 3)
        {
            Instantiate(adultDragons[index], new Vector3(0, 0, 0), Quaternion.identity);
        }
    }
}