using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;

public class DragonBook : MonoBehaviour
{
    [SerializeField] private GameObject dragonCollection;
    [SerializeField] private GameObject closeButton;
    [SerializeField] private GameObject AskToAddDragonPanel;
    [SerializeField] private GameObject AskForDragonCloseButton;
    [SerializeField] private GameObject DragonLimit;
    private static List<string> dragonsInBackyard = new List<string>();
    /*private static List<int> pagesInBook = new List<int>();

    public static List<string> dragonNamesInBook = new List<string>();
    public static List<int> dragonTypesInBook = new List<int>();*/

    public static string dragonName;
    private int limit = 4;

    public void Start()
    {
        dragonCollection.SetActive(false);
        closeButton.SetActive(false);
        AskToAddDragonPanel.SetActive(false);
        AskForDragonCloseButton.SetActive(false);
        DragonLimit.SetActive(false);
    }

    public void ShowDragonCollection()
    {
        dragonCollection.SetActive(true);
        closeButton.SetActive(true);
    }

    public void CloseDragonCollection()
    {
        dragonCollection.SetActive(false);
        closeButton.SetActive(false);
    }

    public void ShowAskToAddDragon()
    {
        AskToAddDragonPanel.SetActive(true);
        AskForDragonCloseButton.SetActive(true);
        closeButton.SetActive(false);
        //dragonName = currentDragonName;
    }

    public void CloseAskToAddDragon()
    {
        AskToAddDragonPanel.SetActive(false);
        AskForDragonCloseButton.SetActive(false);
        closeButton.SetActive(true);
    }

    public void InstantiateDragon()
    {
        //string dragonName = gameObject.transform.GetChild(0).Resources.Load(prefabName);
        //TMP_Text dragonName = gameObject.transform.GetChild(0).GetComponent<TMP_Text>();
        //string newDragonName = dragonName.text;
        //string dragonName = gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text;
        //Debug.Log(dragonName);
        if (dragonsInBackyard.Count >= limit)
        {
            DragonLimit.SetActive(true);
            Invoke("InactivateLimitText", 3f);
        }
        else
        {
            if (dragonName != null && !dragonsInBackyard.Contains(dragonName))
            {
                dragonsInBackyard.Add(dragonName);
                GameObject newDragon = (GameObject)Instantiate(Resources.Load(dragonName));
                CloseDragonCollection();
            }
        }
        //planet = (GameObject)Instantiate(Resources.Load(prefabName))
        //Instantiate(necklaceParticles, pickup.transform.position, Quaternion.identity);
    }

    private void InactivateLimitText()
    {
        DragonLimit.SetActive(false);
    }

    public void RemoveDragon()
    {
        dragonsInBackyard.Remove(dragonName);
        Destroy(GameObject.Find(dragonName + "(Clone)"));
        CloseAskToAddDragon();
    } 
}
