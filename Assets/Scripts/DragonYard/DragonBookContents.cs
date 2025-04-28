using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DragonBookContents : MonoBehaviour
{
    private static DragonBookContents drBookCont;

    private static List<int> pagesInBook = new List<int>();

    private static List<string> dragonNamesInBook = new List<string>();
    private static List<int> dragonTypesInBook = new List<int>();

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

    public static void SetNewDragonNameAndType(string name, int type)
    {
        dragonNamesInBook.Add(name);
        dragonTypesInBook.Add(type);
    }

    public List<string> GetDragonNames()
    {
        return dragonNamesInBook;
    }

    public List<int> GetDragonTypes()
    {
        return dragonTypesInBook;
    }
}