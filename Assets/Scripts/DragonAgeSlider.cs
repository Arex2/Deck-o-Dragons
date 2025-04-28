using UnityEngine;
using UnityEngine.UI;

public class DragonAgeSlider : MonoBehaviour
{
    public Slider slider;
    public DragonController dragonController;

    private void Start()
    {
        slider.value = 0;
    }

    public void IncreaseProgress()
    {
        if (slider.value < slider.maxValue)
        {
            slider.value++;
        }

        if (slider.value >= slider.maxValue)
        {
            slider.value = 0;
            dragonController.SpawnNewDragon();
        }
    }
}