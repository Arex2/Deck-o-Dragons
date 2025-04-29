using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DragonController : MonoBehaviour
{
    public DragonActive dragonActive;
    private Slider evolutionSlider;
    //private TMP_Text statusText;
    //[SerializeField] private GameObject drakPrefab;
    [SerializeField] private Vector2 eggStart = Vector2.zero;

    private void Awake()
    {
        //DontDestroyOnLoad(drakPrefab);
        dragonActive = GameObject.Find("DragonActive").GetComponent<DragonActive>();
        evolutionSlider = GameObject.Find("EvolutionSlider").GetComponent<Slider>();
        //statusText = GameObject.Find("CompleteTraining_Text").GetComponent<TMP_Text>();

        evolutionSlider.minValue = 0;
        evolutionSlider.maxValue = 3;
        evolutionSlider.value = DragonActive.evolutionProcess;

        //statusText.text = "";
        DragonActive.currentDragon = this;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SpawnNewDragon();
        }

        /*if (!DragonActive.isDragonActive)
        {
            statusText.text = "";
        }*/
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
                DragonBookContents.SetNewDragonNameAndType(DragonActive.dragonName, DragonActive.index);
                Invoke("SpawnNewDragon", 6f);
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
            //statusText.text = "";
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