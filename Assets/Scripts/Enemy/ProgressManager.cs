using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    public static int currentLevel = 0;
    public bool WonLastBattle { get; set; }



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

    public void IncreaseLevel()
    {
        currentLevel++;
        WonLastBattle = true;
    }

    public void ResetProgress()
    {
        currentLevel = 0;
    }
}
