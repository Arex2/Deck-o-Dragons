using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DragonActive : MonoBehaviour
{
    [Header ("Script instances")]
    private static DragonActive DrActInstance;
    public static DragonController currentDragon;

    [Header ("Dragon collection lists (prefabs)")]
    [SerializeField] public GameObject egg;
    [SerializeField] public GameObject[] babyDragons;
    [SerializeField] public GameObject[] teenDragons;
    [SerializeField] public GameObject[] adultDragons;

    [Header ("Variables")]
    public static TMP_Text statusText;
    public static string dragonName;
    public static bool isDragonActive;  //används i Egg & TextInputManager
    public static bool doCheck;
    public static int index;            //element
    public static int age;
    public static int evolutionProcess; //sliderprogress

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

        statusText = GameObject.Find("Status_Text").GetComponent<TMP_Text>();
        statusText.text = "";

        doCheck = true;

        DontDestroyOnLoad(this);
    }

    void LateUpdate()
    {
        if(doCheck)
        {
            if(SceneManager.GetActiveScene().buildIndex == 1)
            {
                CheckForEggOrDragon();
                doCheck = false;
                currentDragon = FindObjectOfType<DragonController>();
            }
        }
    }

    public static void StepProgress()
    {
        currentDragon.StepProgress();
    }

    private void CheckForEggOrDragon()
    {
        if(!isDragonActive)
        {
            Instantiate(egg, new UnityEngine.Vector3(0, 0, 0), UnityEngine.Quaternion.identity);
            //TextInputManager.dragonName.text = "";
        }
        else if(isDragonActive)
        {
            //Instantiate(TextInputManager.dragonName, new UnityEngine.Vector3(0, 1000, 0), UnityEngine.Quaternion.identity);
            SpawnDragons();
        }
    }

    public void SpawnDragons()
    {
        if (age == 1)
        {
            Instantiate(babyDragons[index], new UnityEngine.Vector3(0, -3, 0), UnityEngine.Quaternion.identity);
        }
        else if (age == 2)
        {
            Instantiate(teenDragons[index], new UnityEngine.Vector3(0, -3, 0), UnityEngine.Quaternion.identity);
        }
        else if (age == 3)
        {
            Instantiate(adultDragons[index], new UnityEngine.Vector3(0, -4, 0), UnityEngine.Quaternion.identity);
        }
    }
}