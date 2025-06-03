using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActivationManager : MonoBehaviour
{
    private void Awake()
    {
        if (ProgressManager.wonLastBattle)
        {
            ProgressManager.wonLastBattle = false;

            GameObject.Find("ScreenCover").GetComponent<CanvasGroup>().blocksRaycasts = true;

            Invoke("AddProgressToDragon", 1);
        }
    }
 
    private void AddProgressToDragon()
    {
        DragonActive.CurrentDragon.StepProgress();
    }
}
