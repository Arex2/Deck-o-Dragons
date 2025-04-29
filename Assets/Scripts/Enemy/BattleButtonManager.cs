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
        test();
    }
    private void test()
    {
        for (int i = 0; i < buttonList.Count; i++)
        {
            //rita linje mellan
            if(i <= currentLevel)
            {
                drawLine.AddButtonToLine(buttonList[i]);
                continue;
            }

            /*
            if (i == currentLevel)
            {
                continue;
            }
            */

            Button button = buttonList[i];
            ColorBlock colors = button.colors;
            colors.normalColor = Color.black;
            colors.highlightedColor = Color.black;
            colors.pressedColor = Color.black;
            colors.selectedColor = Color.black;
            colors.disabledColor = Color.black;
            button.colors = colors;
            button.interactable = false;

        }
    }
}
