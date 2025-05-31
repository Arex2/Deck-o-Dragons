using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class DragonBook : MonoBehaviour
{
    [SerializeField] private GameObject dragonCollection;
    [SerializeField] private GameObject dragonLimitText;
    [SerializeField] private AudioClip openBookSFX;

    private Book Book;
    [SerializeField] private GameObject ControlledBook;
    [SerializeField] private Image activeLeftPage;
    [SerializeField] private Image activeRightPage;
    public static bool addDragonsToBook = false;

    //[SerializeField] private GameObject AskToAddDragonPanel;
    //[SerializeField] private GameObject AskForDragonCloseButton;

    [Header ("Variables dictating which dragon to affect")]
    public static string dragonName;
    public static int dragonElement;

    [Header("Buttons")]
    [SerializeField] private Button[] buttons;
    [SerializeField] private GameObject closeButton;
    /*[SerializeField] private Button button1;
    [SerializeField] private Button button2;
    [SerializeField] private Button button3;
    [SerializeField] private Button button4;
    [SerializeField] private Button button5;
    [SerializeField] private Button button6;*/

    [Header("Element Types")]
    private string water = "Chibi Water Dragon"; // 0
    private string earth = "Chibi Earth Dragon"; // 1
    private string fire = "Chibi Fire Dragon";   // 2
    private string air = "Chibi Air Dragon";     // 3

    [Header ("Position related to spawning dragons")]
    private Vector3 leftOuterBounds = new Vector3(-1.5f, 4f, 0);
    private Vector3 rightOuterBounds = new Vector3(1.5f, -4f, 0);

    private int currentPage;
    //private static List<string> dragonsInBackyard = new List<string>();
    //private static List<int> dragonElementsInBackyard = new List<int>();
    /*private static List<int> pagesInBook = new List<int>();

    public static List<string> dragonNamesInBook = new List<string>();
    public static List<int> dragonTypesInBook = new List<int>();*/

    //private int limit = 4;

    public void Start()
    {
        DragonBookContents.SetNewDragonNameAndTypeInBook("bobo", 0);
        DragonBookContents.SetNewDragonNameAndTypeInBook("rawr", 1);
        /*SetNewDragonNameAndTypeInBook("0123456789", 2);
        SetNewDragonNameAndTypeInBook("drake", 3);
        SetNewDragonNameAndTypeInBook("hello", 1);
        SetNewDragonNameAndTypeInBook("kjbfkbd", 3);*/

        //dragonsInBackyard.Clear();
        //dragonElementsInBackyard.Clear();
        dragonCollection.SetActive(false);
        closeButton.SetActive(false);
        dragonLimitText.SetActive(false);
        //AskToAddDragonPanel.SetActive(false);
        //AskForDragonCloseButton.SetActive(false);

        //ControlledBook = GameObject.Find("Book");

        /*if (!ControlledBook)
        {
            //ControlledBook = GetComponent<Book>();
            ControlledBook = GameObject.Find("Book").GetComponent<Book>();
        }*/

        //AddDragonsToBookCollectionAPageAtATime();
        AddSelectedDragonsToYardOnLoad();
        AddDragonsToBookPages();
    }

    private void Update()
    {
        DragonBookContents.testprint();

        if(addDragonsToBook)
        {
            AddDragonsToBookPages();
            addDragonsToBook = false;
        }
    }

    private void AddDragonsToBookPages()
    {
        //int indexOfDragonToAdd = currentPage * DragonBookContents.GetDragonsPerPageAmount();
        int indexOfDragonToAdd = (ControlledBook.GetComponent<Book>().currentPage - 2) * DragonBookContents.GetDragonsPerPageAmount();

        for(int i = 0; i < DragonBookContents.GetDragonsPerPageAmount(); i++)
        {
            Button button = activeLeftPage.transform.GetChild(i).GetComponent<Button>();
            //Button rightButton = activeRightPage.transform.GetChild(i).GetComponent<Button>();

            //if (DragonBookContents.GetDragonNamesInBook().Count <= i) //DragonBookContents.GetDragonNames().ElementAt(i) == null)
            if(DragonBookContents.GetDragonNamesInBook().Count <= i || DragonBookContents.GetDragonNamesInBook().Count >= indexOfDragonToAdd)
            {
                button.interactable = false;
                button.transform.GetChild(0).GetComponent<TMP_Text>().text = "";
            }
            else if (DragonBookContents.GetDragonNamesInBook().ElementAt(indexOfDragonToAdd) != null)
            {
                AddDragonLoop(button, indexOfDragonToAdd);
            }

            indexOfDragonToAdd++;
        }

        for (int i = 0; i < DragonBookContents.GetDragonsPerPageAmount(); i++)
        {
            Button button = activeRightPage.transform.GetChild(i).GetComponent<Button>();
            //Button rightButton = activeRightPage.transform.GetChild(i).GetComponent<Button>();

            if (DragonBookContents.GetDragonNamesInBook().Count <= i || DragonBookContents.GetDragonNamesInBook().Count >= indexOfDragonToAdd)
            {
                button.interactable = false;
                button.transform.GetChild(0).GetComponent<TMP_Text>().text = "";
            }
            else if (DragonBookContents.GetDragonNamesInBook().ElementAt(indexOfDragonToAdd) != null)
            {
                AddDragonLoop(button, indexOfDragonToAdd);
            }

            indexOfDragonToAdd++;
        }
    }

    private void AddDragonLoop(Button button, int indexOfDragonToAdd)
    {
        button.interactable = true;
        button.transform.GetChild(0).GetComponent<TMP_Text>().text = DragonBookContents.GetDragonNamesInBook().ElementAt(indexOfDragonToAdd);
        //buttons[j].transform.image = DragonBookContents.GetDragonTypes().ElementAt(i);
        int elementType = DragonBookContents.GetDragonTypesInBook().ElementAt(indexOfDragonToAdd);

        switch (elementType)
        {
            case 0:
                button.GetComponent<OnDragonButtonClick>().SetElementAndElementNumber(water, elementType);  //water type
                break;
            case 1:
                button.GetComponent<OnDragonButtonClick>().SetElementAndElementNumber(earth, elementType);  //earth type
                break;
            case 2:
                button.GetComponent<OnDragonButtonClick>().SetElementAndElementNumber(fire, elementType);   // fire type
                break;
            case 3:
                button.GetComponent<OnDragonButtonClick>().SetElementAndElementNumber(air, elementType);    // air type
                break;
            default:
                break;
        }
    }

    /*private void AddDragonsToBookCollectionAPageAtATime()
    {
        int indexOfDragonToAdd = currentPage * DragonBookContents.GetDragonsPerPageAmount();

        for (int j = 0; j < DragonBookContents.GetDragonsPerPageAmount(); j++) //g� igenom varje knapp på en sida
        {
            if (DragonBookContents.GetDragonNamesInBook().Count <= j) //DragonBookContents.GetDragonNames().ElementAt(i) == null)
            {
                buttons[j].interactable = false;
                buttons[j].transform.GetChild(0).GetComponent<TMP_Text>().text = "";
            }
            else if (DragonBookContents.GetDragonNamesInBook().ElementAt(indexOfDragonToAdd) != null)
            {
                buttons[j].interactable = true;
                buttons[j].transform.GetChild(0).GetComponent<TMP_Text>().text = DragonBookContents.GetDragonNamesInBook().ElementAt(indexOfDragonToAdd);
                //buttons[j].transform.image = DragonBookContents.GetDragonTypes().ElementAt(i);
                int elementType = DragonBookContents.GetDragonTypesInBook().ElementAt(indexOfDragonToAdd);

                switch (elementType)
                {
                    case 0:
                        buttons[j].GetComponent<OnDragonButtonClick>().SetElementAndElementNumber(water, elementType);  //water type
                        break;
                    case 1:
                        buttons[j].GetComponent<OnDragonButtonClick>().SetElementAndElementNumber(earth, elementType);  //earth type
                        break;
                    case 2:
                        buttons[j].GetComponent<OnDragonButtonClick>().SetElementAndElementNumber(fire, elementType);   // fire type
                        break;
                    case 3:
                        buttons[j].GetComponent<OnDragonButtonClick>().SetElementAndElementNumber(air, elementType);    // air type
                        break;
                    default:
                        break;
                }
            }

            indexOfDragonToAdd++;
        }
    }*/

    private void AddSelectedDragonsToYardOnLoad()
    {
        if (DragonBookContents.GetDragonsActiveInBackyard().Count > 0)
        {
            for (int i = 0; i < DragonBookContents.GetDragonsActiveInBackyard().Count; i++)
            {
                dragonName = DragonBookContents.GetDragonsActiveInBackyard().ElementAt(i);
                dragonElement = DragonBookContents.GetElementsOfDragonsActiveInBackyard().ElementAt(i);

                switch (dragonElement)
                {
                    case 0:
                        InstatiateDragon(water);
                        break;
                    case 1:
                        InstatiateDragon(earth);
                        break;
                    case 2:
                        InstatiateDragon(fire);
                        break;
                    case 3:
                        InstatiateDragon(air);
                        break;
                    default:
                        break;
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

    private void InstatiateDragon(string element)
    {
        Vector3 randomSpawnPos = new Vector3(Random.Range(leftOuterBounds.x, rightOuterBounds.x), Random.Range(leftOuterBounds.y, rightOuterBounds.y), 0);
        GameObject newDragon = (GameObject)Instantiate(Resources.Load(element), randomSpawnPos, Quaternion.identity);
        newDragon.name = dragonName;
    }

    public void CheckToInstantiateDragon(string element)
    {
        if (DragonBookContents.GetDragonsActiveInBackyard().Contains(dragonName)) //(DragonBookContents.GetDragonsInYard().Contains(dragonName))
        {
            RemoveDragon();
        }
        else if (DragonBookContents.GetDragonsActiveInBackyard().Count >= DragonBookContents.LimitOfDragons)
        {
            dragonLimitText.SetActive(true);
            Invoke("InactivateLimitText", 3f);
        }
        else
        {
            if (dragonName != null && !DragonBookContents.GetDragonsActiveInBackyard().Contains(dragonName))
            {
                DragonBookContents.SetNewDragonNamesAndElementsActiveInBackyard(dragonName, dragonElement);
                //dragonsInBackyard.Add(dragonName);
                //dragonElementsInBackyard.Add(dragonElement);

                InstatiateDragon(element);
                CloseDragonCollection();
            }
        }
    }

    public void RemoveDragon()
    {
        int indexOfDragonName = DragonBookContents.GetDragonsActiveInBackyard().IndexOf(dragonName);
        DragonBookContents.RemoveDragonAndElementFromBackyard(dragonName, indexOfDragonName);
        Destroy(GameObject.Find(dragonName));

        //dragonsInBackyard.Remove(dragonName);
        //dragonElementsInBackyard.RemoveAt(indexOfDragonName);

        //CloseAskToAddDragon();
    }

    public void ShowDragonCollection()
    {
        AudioManager.Instance.PlaySFX(openBookSFX);
        dragonCollection.SetActive(true);
        closeButton.SetActive(true);
    }

    public void CloseDragonCollection()
    {
        dragonCollection.SetActive(false);
        closeButton.SetActive(false);
    }

    private void InactivateLimitText()
    {
        dragonLimitText.SetActive(false);
    }
}