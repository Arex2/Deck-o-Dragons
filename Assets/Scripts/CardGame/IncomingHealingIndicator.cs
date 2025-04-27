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

    public void ShowIncomingHealing(float incomingHealing, float currentHP)
    {
        healingSlider.value = CalculateHealingToSlider(incomingHealing, currentHP);
    }

    public void hide()
    {
        healingSlider.value = 0;
    }

    private float CalculateHealingToSlider(float healing, float hp)
    {
        //damage to make  /  current hp  =  slider value
        return hp + healing;
    }
}
