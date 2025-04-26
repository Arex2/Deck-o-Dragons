using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    private int currentLevel = 0;

    

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

  public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public void IncreaseLevel()
    {
        currentLevel++;
    }
}
