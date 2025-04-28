using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EncounterManager : MonoBehaviour
{
    [SerializeField] private GameBehaviour gameBehaviour;
    [SerializeField] private List<GameObject> encounters;
    [SerializeField] private Transform enemySpawnLocation;
    public int currentEncounterIndex { get; private set; } = -1;
    public EnemyBoss currentEncounterEnemy { get; private set; }

    void Start()
    {
        //Hakig kod för att få random ordning på fienderna i listan
        encounters = encounters.OrderBy(enemy => Random.Range(1f, 100f)).ToList<GameObject>();
    }
    public void InctanceNextEncounter()
    {
        currentEncounterIndex++;

        if (currentEncounterIndex >= encounters.Count)
        {
            //Increase Progression
            if(ProgressManager.Instance != null)
            {
                ProgressManager.Instance.IncreaseLevel();
                EnemyScalingManager.Instance.AdvanceScaling();
                ActivationManager.Instance.SetWonLastEncounter(true);
            }

            if (ActivationManager.Instance != null)
            {
                ActivationManager.Instance.SetWonLastEncounter(true);
                SceneManager.UnloadSceneAsync(4);
                SceneManager.LoadScene(6, LoadSceneMode.Additive);
            }
            else
            {
                // Card shop scene
                SceneManager.LoadScene(6);
            }


           

        }
        else if(encounters[currentEncounterIndex] != null)
        {
            if(currentEncounterEnemy != null) Destroy(currentEncounterEnemy.gameObject);
            var temp = Instantiate(encounters[currentEncounterIndex], enemySpawnLocation);
            currentEncounterEnemy = temp.GetComponent<EnemyBoss>();
            print(currentEncounterEnemy.ToString());
            //Restart Game Logic
        }
        else
        {
            print("No Enemy found");
        }
    }
}
