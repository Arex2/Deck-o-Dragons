using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OnDragonButtonClick : MonoBehaviour
{
    private Button button;
    private string element;
    private int elementType;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(PassObjectToAnotherScript);
        //button.onClick.AddListener(DragonBook.InstantiateDragon(GetButton());
    }

    private void PassObjectToAnotherScript()
    {
        string dragonName = gameObject.transform.GetChild(0).GetComponent<TMP_Text>().text;
        DragonBook.dragonName = dragonName;
        DragonBook.dragonElement = elementType;
        GameObject.Find("PanelController").GetComponent<DragonBook>().InstantiateDragon(element);
    }

    public void SetElement(string element)
    {
        this.element = element;
    }

    public void SetElementType(int elementType)
    {
        this.elementType = elementType;
    }
}