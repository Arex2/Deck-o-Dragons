using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActivationManager : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("EVOLUTION: " + DragonActive.evolutionProcess);

        if (ProgressManager.Instance != null)
        {
            if (ProgressManager.Instance.WonLastBattle)
            {
                ProgressManager.Instance.WonLastBattle = false;

                GameObject.Find("ScreenCover").GetComponent<CanvasGroup>().blocksRaycasts = true;

                Invoke("AddProgressToDragon", 1);
            }
        }
    }
 
    private void AddProgressToDragon()
    {
        DragonActive.CurrentDragon.StepProgress();
    }
}
