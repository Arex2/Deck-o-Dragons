using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleButtonManager : MonoBehaviour
{
    [SerializeField] DrawLine drawLine;
    [SerializeField] private List<Button> buttonList = new List<Button>();
    private int currentLevel;
    void Start()
    {
        currentLevel = ProgressManager.Instance.GetCurrentLevel();
        //currentLevel = 3;
        Debug.Log("Current lvl: " + currentLevel);
        test();
    }
    private void test()
    {
        for (int i = 0; i < buttonList.Count; i++)
        {
            Button button = buttonList[i];
            button.interactable = false;

            if (i > currentLevel)
            {
                continue;
            }

            drawLine.AddButtonToLine(button);

            if (i == currentLevel)
            {
                button.interactable = true;
                button.image.color = Color.white;
            }
            else
            {
                button.image.color = Color.grey;
            }

            //OLD
            /*
            //rita linje mellan
            if(i <= currentLevel)
            {
                drawLine.AddButtonToLine(buttonList[i]);
                continue;
            }

            Button button = buttonList[i];
            ColorBlock colors = button.colors;
            colors.normalColor = Color.black;
            colors.highlightedColor = Color.black;
            colors.pressedColor = Color.black;
            colors.selectedColor = Color.black;
            colors.disabledColor = Color.black;
            button.colors = colors;
            button.interactable = false;
            */

        }
    }

}
