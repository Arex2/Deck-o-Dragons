using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DragonController : MonoBehaviour
{
    private DragonActive dragonActive;
    private Slider evolutionSlider;
    [SerializeField] private Vector2 eggStart = Vector2.zero;

    private void Awake()
    {
        dragonActive = GameObject.Find("DragonActive").GetComponent<DragonActive>();
        evolutionSlider = GameObject.Find("EvolutionSlider").GetComponent<Slider>();

        evolutionSlider.minValue = 0;
        evolutionSlider.maxValue = 3;
        evolutionSlider.value = DragonActive.evolutionProcess;

        DragonActive.currentDragon = this;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SpawnNewDragon();
        }
    }

    public void StepProgress()
    {
        evolutionSlider.value++;
        DragonActive.evolutionProcess++;

        if (evolutionSlider.value >= 3)
        {
            // Special case for adults
            if (DragonActive.age == 3)
            {
                DragonActive.statusText.text = "Congratulations! " + DragonActive.dragonName + " has completed their training and will be added to your registry!";

                if(DragonActive.dragonName != null)
                {
                    DragonBookContents.SetNewDragonNameAndTypeInBook(DragonActive.dragonName, DragonActive.index);
                }

                if(DragonBookContents.GetDragonsActiveInBackyard().Count < DragonBookContents.LimitOfDragons)
                {
                    DragonBookContents.SetNewDragonNamesAndElementsActiveInBackyard(DragonActive.dragonName, DragonActive.index);
                }

                Invoke("SpawnNewDragon", 4f);
            }
            else
            {
                SpawnNewDragon();
            }

            evolutionSlider.value = 0;
            DragonActive.evolutionProcess = 0;
        }
    }

    public void SpawnNewDragon()
    {
        if(DragonActive.age == 1)
        {
            Instantiate(dragonActive.teenDragons[DragonActive.index], new Vector3(0, -3, 0), Quaternion.identity);
            DragonActive.age++;
        }
        else if (DragonActive.age == 2)
        {
            Instantiate(dragonActive.adultDragons[DragonActive.index], new Vector3(0, -4, 0), Quaternion.identity);
            DragonActive.age++;
        }
        else if (DragonActive.age == 3)
        {
            Instantiate(dragonActive.egg, eggStart, Quaternion.identity);
            DragonActive.age = 0;
        }
        else
        {
            return;
        }

        //Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
        //Instantiate(drakPrefab, drakStart, Quaternion.identity);*/
        Destroy(gameObject);
    }
}