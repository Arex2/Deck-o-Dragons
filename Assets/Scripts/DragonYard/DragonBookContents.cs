using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Rendering.FilterWindow;

public class DragonBookContents : MonoBehaviour
{
    private static DragonBookContents drBookCont;

    [Header ("Dragons shown in book related")]
    private const int DRAGONS_PER_PAGE = 6;
    private static List<int> pagesInBook = new List<int>();
    private static List<string> dragonNamesInBook = new List<string>();
    private static List<int> dragonTypesInBook = new List<int>();

    [Header ("Dragons shown in background related")]
    public static int LimitOfDragons = 8;
    private static List<string> dragonsActiveInBackyard = new List<string>();
    private static List<int> elementsOfDragonsActiveInBackyard = new List<int>();

    private bool buildCheck = true;

    //public bool doCheck;

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
        if (SceneManager.GetActiveScene().buildIndex == 1 && buildCheck)
        {
            Debug.Log("Ägg: NAME SIZE : " + dragonsActiveInBackyard.Count);
            buildCheck = false;
        }

        if (SceneManager.GetActiveScene().buildIndex != 1 && !buildCheck)
        {
            Debug.Log("Contents: NAME SIZE : " + dragonsActiveInBackyard.Count);
            buildCheck = true;
        }

        /*if (doCheck)
        {
            if (SceneManager.GetActiveScene().buildIndex == 5)
            {
                doCheck = false;
                //evolutionSlider = GameObject.Find("EvolutionSlider").GetComponent<Slider>();
                //statusText = GameObject.Find("CompleteTraining_Text").GetComponent<TMP_Text>();

                //currentDragon = FindObjectOfType<DragonController>();
            }
        }*/
    }

    public static void SetNewDragonNameAndType(string name, int type)
    {
        dragonNamesInBook.Add(name);
        dragonTypesInBook.Add(type);
    }

    public static void SetNewDragonNamesAndElementsInBackyard(string name, int element)
    {
        dragonsActiveInBackyard.Add(name);
        elementsOfDragonsActiveInBackyard.Add(element);
        //Debug.Log("Contents: NAME SIZE : " + dragonsInBackyard.Count);
        //Debug.Log("Contents: ELEMENT SIZE: " + elementsOfDragonsInBackyard.Count);
    }

    public static void RemoveDragonAndElementFromBackyard(string name, int indexOfElement)
    {
        //Debug.Log("Contents: NAME INDEX : " + dragonsInBackyard.IndexOf(name));
        //Debug.Log("Contents: ELEMENT INDEX: " + indexOfElement);
        dragonsActiveInBackyard.Remove(name);
        elementsOfDragonsActiveInBackyard.RemoveAt(indexOfElement);
    }

    public static List<string> GetDragonsInYard()
    {
        return dragonsActiveInBackyard;
    }

    public static List<int> GetDragonElementsInYard()
    {
        return elementsOfDragonsActiveInBackyard;
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