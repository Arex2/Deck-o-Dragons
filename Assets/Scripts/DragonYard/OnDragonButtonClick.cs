using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OnDragonButtonClick : MonoBehaviour
{
    private Button button;
    private string element;
    private int elementNumber;

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
        DragonBook.dragonElement = elementNumber;
        GameObject.Find("PanelController").GetComponent<DragonBook>().CheckToInstantiateDragon(button, element);
    }

    public void SetElementAndElementNumber(string element, int elementNumber)
    {
        this.element = element;
        this.elementNumber = elementNumber;
    }

    public void OnClick()
    {
        AudioManager.Instance.PlayClickSound();
    }
}