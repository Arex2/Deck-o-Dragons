using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DragonBookContents : MonoBehaviour
{
    private const int DRAGONS_PER_PAGE = 6;
    private static DragonBookContents drBookCont;

    private static List<int> pagesInBook = new List<int>();

    private static List<string> dragonNamesInBook = new List<string>();
    private static List<int> dragonTypesInBook = new List<int>();

    public bool doCheck;

    void Awake()
    {
        if (drBookCont == null)
        {
            drBookCont = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
    }

    void LateUpdate()
    {
        if (doCheck)
        {
            if (SceneManager.GetActiveScene().buildIndex == 5)
            {
                doCheck = false;
                /*evolutionSlider = GameObject.Find("EvolutionSlider").GetComponent<Slider>();
                statusText = GameObject.Find("CompleteTraining_Text").GetComponent<TMP_Text>();*/

                //currentDragon = FindObjectOfType<DragonController>();
            }
        }
    }

    public static void SetNewDragonNameAndType(string name, int type)
    {
        dragonNamesInBook.Add(name);
        dragonTypesInBook.Add(type);
    }

    public static List<string> GetDragonNames()
    {
        return dragonNamesInBook;
    }

    public static List<int> GetDragonTypes()
    {
        return dragonTypesInBook;
    }
}