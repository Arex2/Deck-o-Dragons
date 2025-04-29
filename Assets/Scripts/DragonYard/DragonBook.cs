using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DragonBook : MonoBehaviour
{
    [SerializeField] private GameObject dragonCollection;
    [SerializeField] private GameObject closeButton;
    //[SerializeField] private GameObject AskToAddDragonPanel;
    //[SerializeField] private GameObject AskForDragonCloseButton;
    [SerializeField] private GameObject DragonLimit;

    [Header("Buttons")]
    /*[SerializeField] private Button button1;
    [SerializeField] private Button button2;
    [SerializeField] private Button button3;
    [SerializeField] private Button button4;
    [SerializeField] private Button button5;
    [SerializeField] private Button button6;*/

    [Header("Element Types")]
    string water = "Chibi Water Dragon";
    string earth = "Chibi Earth Dragon";
    string fire = "Chibi Fire Dragon";
    string air = "Chibi Air Dragon";

    [SerializeField] Button[] buttons;

    private static List<string> dragonsInBackyard = new List<string>();
    /*private static List<int> pagesInBook = new List<int>();

    public static List<string> dragonNamesInBook = new List<string>();
    public static List<int> dragonTypesInBook = new List<int>();*/

    public static string dragonName;
    private int limit = 4;
    private int currentPage;

    public void Start()
    {
        dragonCollection.SetActive(false);
        closeButton.SetActive(false);
        //AskToAddDragonPanel.SetActive(false);
        //AskForDragonCloseButton.SetActive(false);
        DragonLimit.SetActive(false);
        AddDragonsToCollection();
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

    public void AddDragonsToCollection()
    {
        DragonBookContents.SetNewDragonNameAndType("bibi", 2);

        //tton1 = dragonCollection.transform.GetChild(0).GetComponent<Button>();
        for (int i = currentPage * 6; i < (currentPage + 1) * 6 - 1; i++) //f� r�tt index f�r drakarna i listorna
        {
            for (int j = 0; j < 6; j++) //g� igenom varje knapp
            {
                if (DragonBookContents.GetDragonNames().Count <= j) //DragonBookContents.GetDragonNames().ElementAt(i) == null)
                {
                    buttons[j].interactable = false;
                    buttons[j].transform.GetChild(0).GetComponent<TMP_Text>().text = "";
                }
                else if(DragonBookContents.GetDragonNames().ElementAt(i) != null)
                {
                    buttons[j].interactable = true;
                    buttons[j].transform.GetChild(0).GetComponent<TMP_Text>().text = DragonBookContents.GetDragonNames().ElementAt(i);
                    //buttons[j].transform.image = DragonBookContents.GetDragonTypes().ElementAt(i);
                    //string elementTYpe = buttons[j].GetComponent<OnDragonButtonClick>().SetElement();
                    int elementType = DragonBookContents.GetDragonTypes().ElementAt(i);

                    switch (elementType)
                    {
                        case 1:
                            buttons[j].GetComponent<OnDragonButtonClick>().SetElement(water); //water type
                            break;
                        case 2:
                            buttons[j].GetComponent<OnDragonButtonClick>().SetElement(earth); //earth type
                            break;
                        case 3:
                            buttons[j].GetComponent<OnDragonButtonClick>().SetElement(fire); // fire type
                            break;
                        case 4:
                            buttons[j].GetComponent<OnDragonButtonClick>().SetElement(air); // air type
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
    
    /*public void ShowAskToAddDragon()
    {
        AskToAddDragonPanel.SetActive(true);
        AskForDragonCloseButton.SetActive(true);
        closeButton.SetActive(false);
        //dragonName = currentDragonName;
    }*/

    /*public void CloseAskToAddDragon()
    {
        AskToAddDragonPanel.SetActive(false);
        AskForDragonCloseButton.SetActive(false);
        closeButton.SetActive(true);
    }*/

    public void InstantiateDragon(string element)
    {
        //string dragonName = gameObject.transform.GetChild(0).Resources.Load(prefabName);
        //TMP_Text dragonName = gameObject.transform.GetChild(0).GetComponent<TMP_Text>();
        //string newDragonName = dragonName.text;
        //string dragonName = gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text;
        //Debug.Log(dragonName);

        if (dragonsInBackyard.Contains(dragonName))
        {
            RemoveDragon();
        }
        else if (dragonsInBackyard.Count >= limit)
        {
            DragonLimit.SetActive(true);
            Invoke("InactivateLimitText", 3f);
        }
        else
        {
            if (dragonName != null && !dragonsInBackyard.Contains(dragonName))
            {
                dragonsInBackyard.Add(dragonName);

                GameObject newDragon = (GameObject)Instantiate(Resources.Load(element));
                newDragon.name = dragonName;
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
        Destroy(GameObject.Find(dragonName));
        //CloseAskToAddDragon();
    } 
}
