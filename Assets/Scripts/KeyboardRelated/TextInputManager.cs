using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class TextInputManager : MonoBehaviour
{
    [SerializeField] public TMP_Text dragonName;
    [SerializeField] private Keyboard keyboard;

    private static GameObject keyboardObject;

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
        if (!DragonActive.isDragonActive)
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
        if (!DragonActive.isDragonActive)
        {
            dragonName.text = "";
        }
        else if (DragonActive.isDragonActive)
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