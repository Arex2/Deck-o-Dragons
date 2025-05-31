using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DragonActive : MonoBehaviour
{
    // Script instances
    public static DragonActive Instance { get; private set; }
    public static DragonController CurrentDragon { get; set; }

    [Header ("Dragon collection lists (prefabs)")]
    [SerializeField] public GameObject egg;
    [SerializeField] public GameObject[] babyDragons;
    [SerializeField] public GameObject[] teenDragons;
    [SerializeField] public GameObject[] adultDragons;

    [Header ("Variables")]
    public static TMP_Text statusText;
    public static string dragonName;
    public static bool isDragonActive;  //används i Egg & TextInputManager
    public static bool doCheck;         //används i         SetNewDragonNameAndTypeInBook("bobo", 1);
    public static int index;            //element
    public static int age;
    public static int evolutionProcess; //sliderprogress

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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
                CurrentDragon = FindObjectOfType<DragonController>();
            }
        }
    }

    public static void StepProgress()
    {
        if(dragonName != null)
        {
            CurrentDragon.StepProgress();
        }
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
            SpawnDragon();
        }
    }

    public GameObject SpawnDragon()
    {
        if (age == 1)
        {
            return Instantiate(babyDragons[index], new UnityEngine.Vector3(0, -3, 0), UnityEngine.Quaternion.identity);
        }
        else if (age == 2)
        {
            return Instantiate(teenDragons[index], new UnityEngine.Vector3(0, -3, 0), UnityEngine.Quaternion.identity);
        }
        else if (age == 3)
        {
            return Instantiate(adultDragons[index], new UnityEngine.Vector3(0, -4, 0), UnityEngine.Quaternion.identity);
        }

        return null;
    }
}