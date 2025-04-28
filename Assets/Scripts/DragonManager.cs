using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonManager : MonoBehaviour
{
    public static DragonManager Instance { get; private set; }

    public Dragon Dragon {get; private set;}
    
    void Awake()
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

    public void SetDragon(Dragon dragon)
    {
        this.Dragon = dragon;
    }
}