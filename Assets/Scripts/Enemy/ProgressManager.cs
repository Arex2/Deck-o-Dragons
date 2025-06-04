using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    public static int currentLevel = 0;
    public static int nbrOfEnemies = 0;
    public static bool wonLastBattle;

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
        wonLastBattle = true;
    }
}
