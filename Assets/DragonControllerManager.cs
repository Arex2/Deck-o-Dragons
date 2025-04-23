using UnityEngine;
using UnityEngine.UI;

public class DragonEvolutionManager : MonoBehaviour
{
    public Slider evolutionSlider;
    private Dragon currentDragon;


    public Keyboard keyboard;

    private void Start()
    {

        evolutionSlider.minValue = 0;
        evolutionSlider.maxValue = 3;
        evolutionSlider.wholeNumbers = true;
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
    }

    public void StepProgress()
    {
        evolutionSlider.value++;

        if (evolutionSlider.value >= 3)
        {
            EvolveCurrentDragon();
            evolutionSlider.value = 0;
        }
    }

    private void EvolveCurrentDragon()
    {
        if (currentDragon != null && currentDragon.nextStage != null)
        {
            Vector3 spawnPos = Vector3.zero;


            if (currentDragon.age == 1)  
            {
                spawnPos = new Vector3(0, -3, 0); 
            }
            else if (currentDragon.age == 2)  
            {
                spawnPos = new Vector3(0, -3, 0);  
            }
            else if (currentDragon.age == 3) 
            {
                spawnPos = new Vector3(0, -4, 0);  
            }


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

    private void OnDragonNameConfirmed()
    {
        if (currentDragon != null)
        {

            if (currentDragon.age == 0)  
            {
                currentDragon.transform.position = new Vector3(0, 0, 0);
                Debug.Log("Egg spawned at (0, 0, 0)");
            }

            if (currentDragon.age == 1 || currentDragon.age == 2)  
            {

                currentDragon.transform.position = new Vector3(0, -3, 0);
                Debug.Log("Moved baby or teen dragon to (0, -3, 0)");
            }
            else if (currentDragon.age == 3) 
            {

                currentDragon.transform.position = new Vector3(0, -4, 0);
                Debug.Log("Moved adult dragon to (0, -4, 0)");
            }
        }
    }
}
