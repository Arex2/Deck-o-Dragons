using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EncounterManager : MonoBehaviour
{
    [SerializeField] private GameBehaviour gameBehaviour;
    [SerializeField] private List<GameObject> encounters;
    [SerializeField] private Transform enemySpawnLocation;
    public int currentEncounterIndex { get; private set; } = -1;
    public GameObject currentEncounterEnemy { get; private set; }

    void Start()
    {
        //Hakig kod för att få random ordning på fienderna i listan
        encounters = encounters.OrderBy(enemy => Random.Range(1f, 100f)).ToList<GameObject>();
    }
    public void InctanceNextEncounter()
    {
        currentEncounterIndex++;
        if(currentEncounterIndex > encounters.Count)
        {
            // You are winner!!!
            return;
        }

        if(encounters[currentEncounterIndex] != null)
        {
            Destroy(currentEncounterEnemy);
            Instantiate(encounters[currentEncounterIndex], enemySpawnLocation);
            //Restart Game Logic
        }
        else
        {
            print("No Enemy found");
        }
    }
}
