using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleButtonManager : MonoBehaviour
{
    [SerializeField] DrawLine drawLine;
    [SerializeField] private List<Button> buttonList = new List<Button>();

    [SerializeField] private Sprite destroyedTowerImage;
    [SerializeField]private Color destroyedColor = Color.white;

    private int currentLevel;


    [Header("Only Testing, doesn't affect actual current level")] 

    private int testLevel = 0;

    [SerializeField]
    public bool progressOneTestLevel; //only for testing

    private void FixedUpdate()
    {
        if (progressOneTestLevel)
        {
            progressOneTestLevel = false;
            testLevel++;
            drawLine.AddButtonToLine(buttonList[testLevel]);

        }

    }


    void Start()
    {
        currentLevel = ProgressManager.Instance.GetCurrentLevel();
        LoadProgressDisplay();
    }
    private void LoadProgressDisplay()
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
            else if (i < currentLevel)
            {
                button.image.sprite = destroyedTowerImage;
                button.image.color = destroyedColor;
            }
            else
            {
                button.image.color = Color.grey;
            }
        }
    }

}
