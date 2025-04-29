using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NativeKeyboardInputManager : MonoBehaviour
{
    private TouchScreenKeyboard keyboard;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (keyboard.active == true)
        {
            Debug.Log("DET SKA FINNAS KEYBOARD");
        }

        //dragonName.text = keyboard.text;
    }

    private void OnGUI()
    {
        //keyboard = TouchScreenKeyboard.Open(nameToEdit, TouchScreenKeyboardType.Default, false, false, false, false, "Please name your dragon", 10);
        keyboard = TouchScreenKeyboard.Open("text to edit");

        /*nameToEdit = GUI.TextField(new Rect(10, 10, 200, 30), nameToEdit, 30);

        if (GUI.Button(new Rect(10, 50, 200, 100), "Default"))
        {
            keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
        }
        if (GUI.Button(new Rect(10, 150, 200, 100), "ASCIICapable"))
        {
            keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.ASCIICapable);
        }
        if (GUI.Button(new Rect(10, 250, 200, 100), "Numbers and Punctuation"))
        {
            keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.NumbersAndPunctuation);
        }*/
    }
}