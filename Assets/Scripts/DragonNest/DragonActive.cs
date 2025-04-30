using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DragonActive : MonoBehaviour
{
    private static DragonActive DrActInstance;

    [SerializeField] public GameObject egg;
    [SerializeField] public GameObject[] babyDragons;
    [SerializeField] public GameObject[] teenDragons;
    [SerializeField] public GameObject[] adultDragons;
    //public DragonActive dragonActive;

    public static DragonController currentDragon;
    //private Slider evolutionSlider;
    public static TMP_Text statusText;
    //public static TMP_Text dragonName;
    //public static GameObject drPref;
    public static string dragonName;
    public static bool isDragonActive; //används i Egg & TextInputManager
    public static bool doCheck;
    public static int index; //element
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

        /*evolutionSlider.minValue = 0;
        evolutionSlider.maxValue = 3;
        evolutionSlider.value = 0;*/

        statusText = GameObject.Find("Status_Text").GetComponent<TMP_Text>();
        statusText.text = "";

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
        if(doCheck)
        {
            if(SceneManager.GetActiveScene().buildIndex == 1)
            {
                CheckForEggOrDragon();
                doCheck = false;
                /*evolutionSlider = GameObject.Find("EvolutionSlider").GetComponent<Slider>();
                statusText = GameObject.Find("CompleteTraining_Text").GetComponent<TMP_Text>();*/

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
            //Egg.SpawnEgg(egg);
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