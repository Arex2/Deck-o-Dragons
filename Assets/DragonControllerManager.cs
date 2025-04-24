using UnityEngine;
using UnityEngine.UI;

public class DragonEvolutionManager : MonoBehaviour
{
    public Slider evolutionSlider;
    public Text statusText; //  assign in the Inspector to show message
    private Dragon currentDragon;
    private bool adultReadyToLayEgg = false; // new flag

    public Keyboard keyboard;

    private void Start()
    {
        evolutionSlider.minValue = 0;
        evolutionSlider.maxValue = 3;
        evolutionSlider.value = 0;

        currentDragon = FindObjectOfType<Dragon>();

        if (keyboard != null)
        {
            keyboard.onEnterPressed += OnDragonNameConfirmed;
        }
    }

    public void SetCurrentDragon(Dragon newDragon)
    {
        currentDragon = newDragon;
        adultReadyToLayEgg = false;

        // Reset text visibility
        statusText.text = "";
        statusText.gameObject.SetActive(false);
    }


    public void StepProgress()
    {
        evolutionSlider.value++;

        if (evolutionSlider.value >= 3)
        {
            // Special case for adults
            if (currentDragon != null && currentDragon.CompareTag("Adult"))
            {
                if (!adultReadyToLayEgg)
                {
                    statusText.text = "The dragon is ready to pass on its legacy...";
                    statusText.gameObject.SetActive(true);
                    adultReadyToLayEgg = true;
                    return;
                }

                else
                {
                    SpawnEggAndDestroyAdult();
                    evolutionSlider.value = 0;
                    return;
                }
            }

            EvolveCurrentDragon();
            evolutionSlider.value = 0;
        }
    }

    private void EvolveCurrentDragon()
    {
        if (currentDragon != null && currentDragon.nextStage != null)
        {
            Vector3 spawnPos = new Vector3(0, currentDragon.age >= 3 ? -4 : -3, 0);

            GameObject oldDragonGO = currentDragon.gameObject;
            GameObject nextDragonGO = Instantiate(currentDragon.nextStage, spawnPos, Quaternion.identity);

            currentDragon = nextDragonGO.GetComponent<Dragon>();

            Destroy(oldDragonGO);
        }
        else
        {
            Debug.LogWarning("No nextStage set on current dragon!");
        }
    }

    private void SpawnEggAndDestroyAdult()
    {
        Vector3 spawnPos = new Vector3(0, 0, 0); // egg position

        if (currentDragon != null && currentDragon.nextStage != null)
        {
            GameObject newEgg = Instantiate(currentDragon.nextStage, spawnPos, Quaternion.identity);
            Debug.Log("A new egg has been spawned!");

            Destroy(currentDragon.gameObject);
            currentDragon = newEgg.GetComponent<Dragon>();
        }

        adultReadyToLayEgg = false;
        statusText.text = "";
        statusText.gameObject.SetActive(false);
    }

    private void OnDragonNameConfirmed()
    {
        if (currentDragon != null)
        {
            if (currentDragon.age == 0)
            {
                currentDragon.transform.position = new Vector3(0, 0, 0);
            }
            else if (currentDragon.age == 1 || currentDragon.age == 2)
            {
                currentDragon.transform.position = new Vector3(0, -3, 0);
            }
            else if (currentDragon.age == 3)
            {
                currentDragon.transform.position = new Vector3(0, -4, 0);
            }
        }
    }
}
