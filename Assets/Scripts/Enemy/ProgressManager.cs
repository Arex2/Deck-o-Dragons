using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    private int currentLevel = 0;
    private bool wonLastBattle;

    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public bool GetWonLastBattle()
    {
        return wonLastBattle;
    }
    public void SetWonLastBattle(bool value)
    {
        wonLastBattle = value;
    }


    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public void IncreaseLevel()
    {
        currentLevel++;
        wonLastBattle = true;
    }
}
