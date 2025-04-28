using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActivationManager : MonoBehaviour
{

    public static ActivationManager Instance { get; private set; }
    private bool wonLastEncounter;

    [SerializeField] private DragonEvolutionManager dragonEvolutionManager;


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

    public void SetWonLastEncounter(bool value)
    {
        wonLastEncounter = value;
    }
    public void DeactivateAllObjects()
    {
        Scene scene = SceneManager.GetActiveScene();
        GameObject[] rootObjects = scene.GetRootGameObjects();

        foreach (GameObject obj in rootObjects)
        {
            obj.SetActive(false);
        }
    }

    public void ActivateAllObjects()
    {
        Scene scene = SceneManager.GetActiveScene();
        GameObject[] rootObjects = scene.GetRootGameObjects();

        foreach (GameObject obj in rootObjects)
        {
            obj.SetActive(true);
        }

        if (wonLastEncounter)
        {
            Invoke("AddProgressToDragon", 1);
            wonLastEncounter = false;
        }
        else
        {
            //Dragon should take damage here
        }
    }

    private void AddProgressToDragon()
    {
        dragonEvolutionManager.StepProgress();
    }
}
