using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleButtonManager : MonoBehaviour
{

    [SerializeField] private List<Button> buttonList = new List<Button>();
    private int currentLevel;
    void Start()
    {
        currentLevel = ProgressManager.Instance.GetCurrentLevel();
        test();
    }
    private void test()
    {
        for (int i = 0; i < buttonList.Count; i++)
        {
            if (i == currentLevel)
                continue;

            Button button = buttonList[i];
            ColorBlock colors = button.colors;
            colors.normalColor = Color.grey;
            colors.highlightedColor = Color.grey;
            colors.pressedColor = Color.grey;
            colors.selectedColor = Color.grey;
            colors.disabledColor = Color.grey;
            button.colors = colors;
            button.interactable = false;
        }
    }
}
