using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using static UnityEditor.Rendering.FilterWindow;

public class DragonBookContents : MonoBehaviour
{
    private static DragonBookContents drBookCont;

    [Header ("Dragons shown in book related")]
    private const int DRAGONS_PER_PAGE = 3;
    private static List<int> pagesInBook = new List<int>();
    private static List<string> dragonNamesInBook = new List<string>();
    private static List<int> dragonTypesInBook = new List<int>();

    [Header ("Dragons shown in background related")]
    public static int LimitOfDragons = 8;
    private static List<string> dragonsActiveInBackyard = new List<string>();
    private static List<int> elementsOfDragonsActiveInBackyard = new List<int>();

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

        SetNewDragonNameAndTypeInBook("bobo", 0);
        SetNewDragonNameAndTypeInBook("rawr", 1);
        SetNewDragonNameAndTypeInBook("0123456789", 2);
        SetNewDragonNameAndTypeInBook("drake", 3);
        SetNewDragonNameAndTypeInBook("hello", 1);
        SetNewDragonNameAndTypeInBook("kjbfkbd", 3);
    }

    public static void SetNewDragonNameAndTypeInBook(string name, int type) //Används i DragonController
    {
        dragonNamesInBook.Add(name);
        dragonTypesInBook.Add(type);
    }

    public static void SetNewDragonNamesAndElementsActiveInBackyard(string name, int element) //Används i DragonBook && DragonController
    {
        dragonsActiveInBackyard.Add(name);
        elementsOfDragonsActiveInBackyard.Add(element);
    }

    public static void RemoveDragonAndElementFromBackyard(string name, int indexOfElement) //Används i DragonBook
    {
        dragonsActiveInBackyard.Remove(name);
        elementsOfDragonsActiveInBackyard.RemoveAt(indexOfElement);
    }

    public static List<string> GetDragonsActiveInBackyard() //Används i DragonBook && DragonController
    {
        return dragonsActiveInBackyard;
    }

    public static List<int> GetElementsOfDragonsActiveInBackyard() //Används i DragonBook
    {
        return elementsOfDragonsActiveInBackyard;
    }

    public static List<string> GetDragonNamesInBook() //Används i DragonBook && NativeKeyboardInputManager
    {

        return dragonNamesInBook;
    }

    public static void testprint()
    {
        foreach (string name in dragonNamesInBook)
        {
            Debug.Log(name);
        }
    }

    public static List<int> GetDragonTypesInBook() //Används i DragonBook
    {
        return dragonTypesInBook;
    }

    public static int GetDragonsPerPageAmount()
    {
        return DRAGONS_PER_PAGE;
    }

    /*public static void toString()
    {
        string names = "Names: ";
        string elements = "Elements: ";

        foreach (string name in GetDragonsActiveInBackyard())
        {
            names += name + ", ";
        }
        foreach (int element in GetElementsOfDragonsActiveInBackyard())
        {
            elements += element.ToString() + ", ";
        }

        Debug.Log(names);
        Debug.Log(elements);
    }*/
}