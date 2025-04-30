using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActivationManager : MonoBehaviour
{


    private void Awake()
    {
        if (ProgressManager.Instance != null)
        {
            if (ProgressManager.Instance.WonLastBattle)
            {
                ProgressManager.Instance.WonLastBattle = false;
                Invoke("AddProgressToDragon", 1);
            }
        }
    }
 
 

    private void AddProgressToDragon()
    {
        DragonActive.StepProgress();
    }
}
