using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IncomingHealingIndicator : MonoBehaviour
{

    [SerializeField]
    private Slider healingSlider;
    [SerializeField]
    private Slider hpSlider;

    [SerializeField]
    float incomingHealing;

    public void Test()
    {
        ShowIncomingHealing(incomingHealing / 10, hpSlider.value);
    }

    public void ShowIncomingHealing(float currentHP, float incomingHealing)
    {
        healingSlider.value = CalculateHealingToSlider(currentHP, incomingHealing);
    }

    public void hide()
    {
        healingSlider.value = 0;
    }

    private float CalculateHealingToSlider(float hp, float healing)
    {
        return (hp + healing) /10;
    }
}
