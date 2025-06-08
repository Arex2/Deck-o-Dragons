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

    [Header("Images")]
    [SerializeField] private Image activeLeftPage;
    [SerializeField] private Image activeRightPage;
    [SerializeField] private Image leftPageOnTurn;
    [SerializeField] private Image rightPageOnTurn;

    [Header("Sprites")]
    [SerializeField] private Sprite waterProfile;
    [SerializeField] private Sprite earthProfile;
    [SerializeField] private Sprite fireProfile;
    [SerializeField] private Sprite airProfile;
    [SerializeField] private Sprite noProfile;

    [Header("Book Rewlated")]
    [SerializeField] private GameObject controlledBook;
    public static bool addDragonsToBook;
    public static bool addDragonsToBookOnFlipPage;
    public static bool pageRelease;

    //[SerializeField] private GameObject AskToAddDragonPanel;
    //[SerializeField] private GameObject AskForDragonCloseButton;

    [Header ("Variables dictating which dragon to affect")]
    public static string dragonName;
    public static int dragonElement;
    public static bool updatePagesAfterFlip;
    public static bool firstFlipCheck;

    [Header("Buttons")]
    [SerializeField] private GameObject closeButton;
    //[SerializeField] private Button[] buttons;
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

    private void Awake()
    {
        //Kod ifall man vill lägga till testdrakar

        /*DragonBookContents.SetNewDragonNameAndTypeInBook("drake", 3);
        DragonBookContents.SetNewDragonNameAndTypeInBook("hello", 1);
        DragonBookContents.SetNewDragonNameAndTypeInBook("new 1", 2);
        DragonBookContents.SetNewDragonNameAndTypeInBook("2is", 3);
        DragonBookContents.SetNewDragonNameAndTypeInBook("tree", 1);
        DragonBookContents.SetNewDragonNameAndTypeInBook("flour", 0);
        DragonBookContents.SetNewDragonNameAndTypeInBook("firth", 1);
        DragonBookContents.SetNewDragonNameAndTypeInBook("sith", 0);*/
    }

    public void Start()
    {
        dragonCollection.SetActive(false);
        closeButton.SetActive(false);
        dragonLimitText.SetActive(false);
        addDragonsToBook = false;
        addDragonsToBookOnFlipPage = false;
        pageRelease = false;
        //AskToAddDragonPanel.SetActive(false);
        //AskForDragonCloseButton.SetActive(false);

        //AddDragonsToBookCollectionAPageAtATime();
        AddSelectedDragonsToYardOnLoad();
        AddDragonsToOpenBookPages();
    }

    private void Update()
    {
        if(pageRelease)
        {
            CopyOnePageToAnother(leftPageOnTurn, activeRightPage, rightPageOnTurn, activeLeftPage);
            pageRelease = false;
        }

        if(addDragonsToBook && !pageRelease)
        {
            addDragonsToBook = false;
            AddDragonsToOpenBookPages();
        }

        if(updatePagesAfterFlip)
        {
            CopyOnePageToAnother(rightPageOnTurn, activeLeftPage, leftPageOnTurn, activeRightPage);
            updatePagesAfterFlip = false;
        }

        if(controlledBook.GetComponent<Book>().pageDragging)
        {
            AddInfoToAllPagesWhenCurrentlyFlippingPages();
        }
    }

    //this method is based on FlipMode.RightToLeft so any call for this method
    //has to send all pages as parameters but with those affected by RTL first
    private void CopyOnePageToAnother(Image RTLpageOnTurn, Image RTLactivePage, Image LTRpageOnTurn, Image LTRactivePage)
    {
        for (int i = 0; i < DragonBookContents.GetDragonsPerPageAmount(); i++)
        {
            if (controlledBook.GetComponent<Book>().mode == FlipMode.RightToLeft)
            {
                Button[] buttonChildren = RTLpageOnTurn.transform.GetComponentsInChildren<Button>();
                Button onFlipButton = buttonChildren[i].GetComponent<Button>();
                Button activeButton = RTLactivePage.transform.GetChild(i).GetComponent<Button>();

                activeButton.interactable = onFlipButton.interactable;
                activeButton.transform.GetChild(0).GetComponent<TMP_Text>().text = onFlipButton.transform.GetChild(0).GetComponent<TMP_Text>().text;
                activeButton.GetComponent<Image>().sprite = onFlipButton.GetComponent<Image>().sprite;
                activeButton.GetComponent<Image>().color = onFlipButton.GetComponent<Image>().color;
            }
            else
            {
                Button[] buttonChildren = LTRpageOnTurn.transform.GetComponentsInChildren<Button>();
                Button onFlipButton = buttonChildren[i].GetComponent<Button>();
                Button activeButton = LTRactivePage.transform.GetChild(i).GetComponent<Button>();

                activeButton.interactable = onFlipButton.interactable;
                activeButton.transform.GetChild(0).GetComponent<TMP_Text>().text = onFlipButton.transform.GetChild(0).GetComponent<TMP_Text>().text;
                activeButton.GetComponent<Image>().sprite = onFlipButton.GetComponent<Image>().sprite;
                activeButton.GetComponent<Image>().color = onFlipButton.GetComponent<Image>().color;
            }
        }
    }

    private void AddInfoToAllPagesWhenCurrentlyFlippingPages()
    {
        if (addDragonsToBookOnFlipPage)
        {
            if (controlledBook.GetComponent<Book>().mode == FlipMode.RightToLeft)
            {
                //activeLeftPage = normal;

                //activeRightPage = currentPage += 2;
                int indexOfDragonToAdd = (controlledBook.GetComponent<Book>().currentPage + 1) * DragonBookContents.GetDragonsPerPageAmount();
                AddButtonsToAPage(activeRightPage, indexOfDragonToAdd);

                //leftPageOnTurn = activeRightPage;
                indexOfDragonToAdd = (controlledBook.GetComponent<Book>().currentPage - 1) * DragonBookContents.GetDragonsPerPageAmount();
                AddButtonsToAPage(leftPageOnTurn, indexOfDragonToAdd);

                //rightPageOnTurn = currentPage += 1;
                indexOfDragonToAdd = (controlledBook.GetComponent<Book>().currentPage) * DragonBookContents.GetDragonsPerPageAmount();
                AddButtonsToAPage(rightPageOnTurn, indexOfDragonToAdd);
            }
            else
            {
                //activeRightPage = normal;

                //activeLeftPage = currentPage -= 4;
                int indexOfDragonToAdd = (controlledBook.GetComponent<Book>().currentPage - 4) * DragonBookContents.GetDragonsPerPageAmount();
                AddButtonsToAPage(activeLeftPage, indexOfDragonToAdd);

                //rightPageOnTurn = activeLeftPage;
                indexOfDragonToAdd = (controlledBook.GetComponent<Book>().currentPage - 2) * DragonBookContents.GetDragonsPerPageAmount();
                AddButtonsToAPage(rightPageOnTurn, indexOfDragonToAdd);

                //leftPageOnTurn = currentPage -= 3;
                indexOfDragonToAdd = (controlledBook.GetComponent<Book>().currentPage - 3) * DragonBookContents.GetDragonsPerPageAmount();
                AddButtonsToAPage(leftPageOnTurn, indexOfDragonToAdd);
            }

            addDragonsToBookOnFlipPage = false;
        }
    }

    private void AddDragonsToOpenBookPages()
    {
        if(controlledBook.GetComponent<Book>().currentPage != 0)
        {
            //.Log(" buttons changing, at: " + Time.deltaTime);

            int indexOfDragonToAdd = (controlledBook.GetComponent<Book>().currentPage - 2) * DragonBookContents.GetDragonsPerPageAmount();
            AddButtonsToAPage(activeLeftPage, indexOfDragonToAdd);

            indexOfDragonToAdd = (controlledBook.GetComponent<Book>().currentPage - 1) * DragonBookContents.GetDragonsPerPageAmount();
            AddButtonsToAPage(activeRightPage, indexOfDragonToAdd);
        }
    }

    private void AddButtonsToAPage(Image page, int indexOfDragonToAdd)
    {
        for (int i = 0; i < DragonBookContents.GetDragonsPerPageAmount(); i++)
        {
            Button button;

            if (page == leftPageOnTurn || page == rightPageOnTurn)
            {
                Button[] buttonChildren = page.transform.GetComponentsInChildren<Button>();
                button = buttonChildren[i].GetComponent<Button>();
                //button = page.transform.GetChild(i + 1).GetComponent<Button>();
            }
            else
            {
                button = page.transform.GetChild(i).GetComponent<Button>();
            }

            if (DragonBookContents.GetDragonNamesInBook().Count <= i || DragonBookContents.GetDragonNamesInBook().Count <= indexOfDragonToAdd)
            {
                button.interactable = false;
                button.transform.GetChild(0).GetComponent<TMP_Text>().text = "";
                button.GetComponent<Image>().sprite = noProfile;
                button.GetComponent<Image>().color = Color.white;

            }
            else if (DragonBookContents.GetDragonNamesInBook().ElementAt(indexOfDragonToAdd) != null)
            {
                AddDragonInfoToButtons(button, indexOfDragonToAdd);
            }

            indexOfDragonToAdd++;
        }
    }

    private void AddDragonInfoToButtons(Button button, int indexOfDragonToAdd)
    {
        button.interactable = true;
        button.transform.GetChild(0).GetComponent<TMP_Text>().text = DragonBookContents.GetDragonNamesInBook().ElementAt(indexOfDragonToAdd);
        dragonName = DragonBookContents.GetDragonNamesInBook().ElementAt(indexOfDragonToAdd);
        //buttons[j].transform.image = DragonBookContents.GetDragonTypes().ElementAt(i);
        int elementType = DragonBookContents.GetDragonTypesInBook().ElementAt(indexOfDragonToAdd);

        switch (elementType)
        {
            case 0:
                button.GetComponent<OnDragonButtonClick>().SetElementAndElementNumber(water, elementType);  //water type
                button.GetComponent<Image>().sprite = waterProfile;
                changeColorOfActiveDragonButtons(button);
                break;
            case 1:
                button.GetComponent<OnDragonButtonClick>().SetElementAndElementNumber(earth, elementType);  //earth type
                button.GetComponent<Image>().sprite = earthProfile;
                changeColorOfActiveDragonButtons(button);
                break;
            case 2:
                button.GetComponent<OnDragonButtonClick>().SetElementAndElementNumber(fire, elementType);   // fire type
                button.GetComponent<Image>().sprite = fireProfile;
                changeColorOfActiveDragonButtons(button);
                break;
            case 3:
                button.GetComponent<OnDragonButtonClick>().SetElementAndElementNumber(air, elementType);    // air type
                button.GetComponent<Image>().sprite = airProfile;
                changeColorOfActiveDragonButtons(button);
                break;
            default:
                break;
        }
    }

    //gammalt script för bok som fungerar annorlunda
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

    private void changeColorOfActiveDragonButtons(Button button)
    {
        if (DragonBookContents.GetDragonsActiveInBackyard().Contains(dragonName))
        {
            button.GetComponent<Image>().color = new Color32(188, 188, 188, 255);
        }
        else
        {
            button.GetComponent<Image>().color = Color.white;
        }
    }

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

    public void CheckToInstantiateDragon(Button button, string element)
    {
        if (DragonBookContents.GetDragonsActiveInBackyard().Contains(dragonName)) //(DragonBookContents.GetDragonsInYard().Contains(dragonName))
        {
            RemoveDragon();
            changeColorOfActiveDragonButtons(button);
        }
        else if (DragonBookContents.GetDragonsActiveInBackyard().Count >= DragonBookContents.LimitOfDragons)
        {
            CancelInvoke();
            dragonLimitText.SetActive(true);
            dragonLimitText.transform.GetChild(0).GetComponent<TMP_Text>().text = "No more than " + DragonBookContents.LimitOfDragons + " dragons at a time can visit the backyard";
            Invoke("InactivateLimitText", 3f);
        }
        else
        {
            if (dragonName != null && !DragonBookContents.GetDragonsActiveInBackyard().Contains(dragonName))
            {
                DragonBookContents.SetNewDragonNamesAndElementsActiveInBackyard(dragonName, dragonElement);
                changeColorOfActiveDragonButtons(button);

                InstatiateDragon(element);
                //CloseDragonCollection();
            }
        }
    }

    public void RemoveDragon()
    {
        int indexOfDragonName = DragonBookContents.GetDragonsActiveInBackyard().IndexOf(dragonName);
        DragonBookContents.RemoveDragonAndElementFromBackyard(dragonName, indexOfDragonName);
        Destroy(GameObject.Find(dragonName));

        //CloseAskToAddDragon();
    }

    public void ShowDragonCollection()
    {
        AudioManager.Instance.PlaySFX(openBookSFX);
        dragonCollection.SetActive(true);
        closeButton.SetActive(true);
        DragonBehavior.canMakeNoise = false;
    }

    public void CloseDragonCollection()
    {
        dragonCollection.SetActive(false);
        closeButton.SetActive(false);
        DragonBehavior.canMakeNoise = true;
    }

    private void InactivateLimitText()
    {
        dragonLimitText.SetActive(false);
    }
}