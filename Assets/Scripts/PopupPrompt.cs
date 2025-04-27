using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupPrompt : MonoBehaviour
{
    [SerializeField]
    GameObject popup;
    [SerializeField]
    TMP_Text textObject;

    [SerializeField]
    string text;

    //game object to center on?

    public PopupPrompt(string text)
    {
        ChangeText(text);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowPopup()
    {
        UpdateText();
        popup.SetActive(true);
    }

    public void HidePopup()
    {
        popup.SetActive(false);
    }
    public void ChangeText(string newText)
    {
        text = newText;
        textObject.text = text;
    }

    private void UpdateText()
    { textObject.text = text; }
}
