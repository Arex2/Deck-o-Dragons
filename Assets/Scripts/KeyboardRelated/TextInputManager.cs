using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextInputManager : MonoBehaviour
{
    [SerializeField] public TMP_Text dragonName;
    [SerializeField] private Keyboard keyboard;

    private static GameObject keyboardObject;

    // Start is called before the first frame update
    void Start()
    {
        keyboard.onKeyPressed += KeyPressedCallback;
        keyboard.onBackspacePressed += BackspacePressedCallback;
        keyboard.onEnterPressed += EnterPressedCallback;
        keyboardObject = GameObject.Find("Keyboard");

        keyboardObject.SetActive(false);
        SpawnName();
    }

    private void Update()
    {
        if (!DragonActive.dragonActive)
        {
            dragonName.text = "";
        }
    }

    public void SpawnKeyboard()
    {
        keyboardObject.SetActive(true);
    }
    private void SpawnName()
    {
        if (!DragonActive.dragonActive)
        {
            dragonName.text = "";
        }
        else if (DragonActive.dragonActive)
        {
            dragonName.text = DragonActive.dragonName;
        }
    }


    private void KeyPressedCallback(char key)
    {
        if (dragonName.text.Length <= 12)
        {
            dragonName.text += key.ToString();
        }
    }

    private void BackspacePressedCallback()
    {
        if (dragonName.text.Length > 0)
        {
            dragonName.text = dragonName.text.Substring(0, dragonName.text.Length - 1);
        }
    }

    private void EnterPressedCallback()
    {
        keyboardObject.SetActive(false);
        DragonActive.dragonName = dragonName.text;
    }
}
