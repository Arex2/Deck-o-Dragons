using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class NativeKeyboardInputManager : MonoBehaviour
{
    [SerializeField] public TMP_Text dragonName;
    private TouchScreenKeyboard keyboard;
    private bool placeName;
    public bool forCompUse;

    void Start()
    {
        placeName = true;
        dragonName.text = DragonActive.dragonName;

        if (!DragonActive.isDragonActive)
        {
            dragonName.text = "";
        }
    }

    void Update()
    {
        if (forCompUse)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                int randomLength = Random.Range(2, 9);
                string allLetters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

                for (int i = 0; i < randomLength; i++)
                {
                    dragonName.text += allLetters[Random.Range(0, allLetters.Length)];
                }

                forCompUse = false;
                DragonActive.dragonName = dragonName.text;
            }
        }

        if (keyboard != null && keyboard.active == true || keyboard != null && TouchScreenKeyboard.visible == true)
        {
            dragonName.text = keyboard.text.Trim();
        }

        if(keyboard != null && keyboard.status == TouchScreenKeyboard.Status.Canceled)
        {
            DragonActive.statusText.text = "Dragon must have a name";
            OpenKeyboard();
            Invoke("ClearStatusText", 1.5f);
        }

        if(placeName && keyboard != null && keyboard.status == TouchScreenKeyboard.Status.Done)
        {
            foreach(string alredayExistingName in DragonBookContents.GetDragonNamesInBook())
            {
                if(dragonName.text == alredayExistingName)
                {
                    DragonActive.statusText.text = "You already have a dragon by that name";
                    OpenKeyboard();
                    Invoke("ClearStatusText", 1.5f);
                    return;
                }
            }

            if(dragonName.text == "".Trim())
            {
                DragonActive.statusText.text = "Dragon must have a name";
                OpenKeyboard();
                Invoke("ClearStatusText", 1.5f);
            }
            else
            {
                placeName = false;
                DragonActive.dragonName = dragonName.text;
            }
        }
    }

    private void ClearStatusText()
    {
        DragonActive.statusText.text = "";
    }

    public void OpenKeyboard()
    {
        DragonActive.statusText.text = "Please name your dragon";
        Invoke("ClearStatusText", 1.5f);
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, true, "Please name your dragon", 10);
    }
}