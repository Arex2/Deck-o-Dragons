using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Key : MonoBehaviour
{
    [SerializeField] private TMP_Text keyText;
    private char key;
    [SerializeField] private int typeOfKey;

    public void SetKey(char key)
    {
        keyText.text = key.ToString();
        this.key = key;
    }

    public Button GetButton()
    {
        return GetComponent<Button>();
    }

    public int TypeOfKey()
    {
        return typeOfKey;
    }
}