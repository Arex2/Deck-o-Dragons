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
    public static bool doCheck;
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

        doCheck = true;

        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        /*if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            Debug.Log("Buildindex är 1");

            if (!DragonActive.dragonActive)
            {
                Instantiate(egg, new Vector3(0, 0, 0), Quaternion.identity);
                //Egg.SpawnEgg(egg);
                Debug.Log("ägg borde finnas");
            }
            else if(DragonActive.dragonActive)
            {
                Debug.Log("drake finns");
                SpawnDragons();
            }
        }*/
    }

    // Update is called once per frame
    void LateUpdate()
    {
        /*if (dragonActive == false)
        {
            DrActInstance = null;
        }*/

        if(doCheck)
        {
            if(SceneManager.GetActiveScene().buildIndex == 1)
            {
                CheckForEggOrDragon();
                doCheck = false;
            }
        }
    }

    private void CheckForEggOrDragon()
    {
        Debug.Log("Buildindex är 1");

        if(!dragonActive)
        {
            Instantiate(egg, new Vector3(0, 0, 0), Quaternion.identity);
            //Egg.SpawnEgg(egg);
            Debug.Log("ägg borde finnas");
        }
        else if(dragonActive)
        {
            Debug.Log("drake finns");
            SpawnDragons();
        }
    }

    public void SpawnDragons()
    {
        if (age == 1)
        {
            Instantiate(babyDragons[index], new Vector3(0, 0, 0), Quaternion.identity);
        }
        else if (age == 2)
        {
            Instantiate(teenDragons[index], new Vector3(0, 0, 0), Quaternion.identity);
        }
        else if (age == 3)
        {
            Instantiate(adultDragons[index], new Vector3(0, 0, 0), Quaternion.identity);
        }
    }
}