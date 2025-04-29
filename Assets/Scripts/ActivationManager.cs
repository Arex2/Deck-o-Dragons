using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActivationManager : MonoBehaviour
{
    [SerializeField] private DragonEvolutionManager dragonEvolutionManager;


    private void Awake()
    {
        if (ProgressManager.Instance != null)
        {
            if (ProgressManager.Instance.GetWonLastBattle())
            {
                ProgressManager.Instance.SetWonLastBattle(false);
                Invoke("AddProgressToDragon", 1);
            }
        }
    }
 
 

    private void AddProgressToDragon()
    {
        dragonEvolutionManager.StepProgress();
    }
}
