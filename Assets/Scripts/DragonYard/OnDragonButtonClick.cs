using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OnDragonButtonClick : MonoBehaviour
{
    //DragonBook drBook; // = new DragonBook();
    private Button button;
    //public GameObject furniture;
    //public Receiver recevier;

    /*public OnDragonButtonClick()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(PassObjectToAnotherScript);
    }*/

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(PassObjectToAnotherScript);
        //button.onClick.AddListener(DragonBook.InstantiateDragon(GetButton());
    }

    /*private Button GetButton()
    {
        return button;
    }*/

    private void PassObjectToAnotherScript()
    {
        //Code to pass the object to another C# script
        //recevier.PassedGameObject = furniture;
        string dragonName = gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text;
        //DragonBook.InstantiateDragon(dragonName);
        //drBook = new DragonBook();
        //drBook.ShowAskToAddDragon();
        GameObject.Find("PanelController").GetComponent<DragonBook>().ShowAskToAddDragon();
        //DragonBook.ShowAskToAddDragon();
        DragonBook.dragonName = dragonName;
        //drBook.InstantiateDragon(dragonName);
        //Invoke("DestroyDragonBook", 0.5f);
    }

    /*private void PassObjectToAnotherScript()
    {
        //Code to pass the object to another C# script
        //recevier.PassedGameObject = furniture;
        string dragonName = gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text;
        //DragonBook.InstantiateDragon(dragonName);
        drBook.InstantiateDragon(dragonName);
        Invoke("DestroyDragonBook", 0.5f);
    }*/

    /*private void DestroyDragonBook()
    {
        Destroy(drBook, 0f);
    }*/
}
