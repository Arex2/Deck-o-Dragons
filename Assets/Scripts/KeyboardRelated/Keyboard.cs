using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class Keyboard : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransf;
    [SerializeField] private Key keyPrefab;
    [SerializeField] private Key backspaceKeyPrefab;
    [SerializeField] private Key enterKeyPrefab;

    [Range(0f, 1f)]
    [SerializeField] private float widthPercent;
    [Range(0f, 1f)]
    [SerializeField] private float heightPercent;
    [Range(0f, 3f)]
    [SerializeField] private float bottomOffSet;

    [SerializeField] private KeyboardLine[] lines;

    [Range(0f, 1f)]
    [SerializeField] private float keyToLineRatio;
    [Range(0f, 1f)]
    [SerializeField] private float keyXSpacing;

    public Action<char> onKeyPressed;
    public Action onBackspacePressed;
    public Action onEnterPressed;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        CreateKeys();

        yield return null;

        UpdateRectTransform();

        //rectTransf.sizeDelta = new Vector2(Screen.width, Screen.height / 2);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRectTransform();
        PlaceKeys();
    }

    private void UpdateRectTransform()
    {
        float width = widthPercent * Screen.width;
        float height = heightPercent * Screen.height;

        //Deciding the size of the keyboard box
        rectTransf.sizeDelta = new Vector2(width, height);

        //Decide the bottom offset
        Vector2 pos;
        pos.x = Screen.width/2;
        pos.y = bottomOffSet * Screen.height + height/2;

        rectTransf.position = pos;
    }

    private void CreateKeys()
    {
        for(int i = 0; i < lines.Length; i++)
        {
            for (int j = 0; j < lines[i].keys.Length; j++)
            {
                char key = lines[i].keys[j];

                if(key == ',') //backspace key
                {
                    Key keyInstance = Instantiate(backspaceKeyPrefab, rectTransf);
                    keyInstance.GetButton().onClick.AddListener(() => BackspacePressedCallback());
                }
                else if(key == '.') //enter key
                {
                    Key keyInstance = Instantiate(enterKeyPrefab, rectTransf);
                    keyInstance.GetButton().onClick.AddListener(() => EnterPressedCallback());
                }
                else
                {
                    Key keyInstance = Instantiate(keyPrefab, rectTransf);
                    keyInstance.SetKey(key);
                    keyInstance.GetButton().onClick.AddListener(() => KeyPressedCallback(key));
                }
            }
        }
    }

    private void PlaceKeys()
    {
        int lineCount = lines.Length;
        float lineHeight = rectTransf.rect.height / lineCount;

        float keyWidth = lineHeight * keyToLineRatio;
        float xSpacing = keyXSpacing * lineHeight;

        int currentKeyIndex = 0;

        for (int i = 0; i < lineCount; i++)
        {
            bool containsBackspace = lines[i].keys.Contains(',');
            bool containsEnter = lines[i].keys.Contains('.');

            float halfKeyCount = (float) lines[i].keys.Length / 2;

            if (containsEnter)
            {
                halfKeyCount += .5f;
            }

            float startX = rectTransf.position.x - (keyWidth + xSpacing) * halfKeyCount + (keyWidth + xSpacing) / 2;
            float lineY = rectTransf.position.y + rectTransf.rect.height / 2 - lineHeight / 2 - i * lineHeight;

            for (int j = 0; j < lines[i].keys.Length; j++)
            {
                bool isEnterKey = lines[i].keys[j] == '.';
                float keyX = startX + j * (keyWidth + xSpacing);

                if (isEnterKey)
                {
                    keyX += keyWidth - xSpacing;
                }

                Vector2 keyPos = new Vector2(keyX, lineY);

                RectTransform keyRectTransform = rectTransf.GetChild(currentKeyIndex).GetComponent<RectTransform>();
                keyRectTransform.position = keyPos;

                float floatKeyWidth = keyWidth;

                if (isEnterKey)
                {
                    floatKeyWidth *= 2;
                }

                keyRectTransform.sizeDelta = new Vector2(floatKeyWidth, keyWidth);

                currentKeyIndex++;
            }
        }
    }

    private void KeyPressedCallback(char key)
    {
        onKeyPressed?.Invoke(key);
    }

    private void BackspacePressedCallback()
    {
        onBackspacePressed?.Invoke();
    }

    private void EnterPressedCallback()
    {
        onEnterPressed?.Invoke();
    }
}

[System.Serializable]
public struct KeyboardLine
{
    public string keys;
}