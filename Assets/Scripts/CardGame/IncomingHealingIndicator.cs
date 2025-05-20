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
    private Image hpFill;

    [SerializeField]
    float incomingHealing;

    public void Test()
    {
        //ShowIncomingHealing(incomingHealing / 10, hpSlider.value);
    }

    public void ShowIncomingHealing(float currentHP, float incomingHealing, float sliderMaxHP)
    {
        healingSlider.normalizedValue = CalculateHealingToSlider(currentHP, incomingHealing, sliderMaxHP);
    }

    public void Hide()
    {
        healingSlider.normalizedValue = 0;
    }

    private float CalculateHealingToSlider(float hp, float healing, float sliderMaxHP)
    {
        //Debug.Log("hp: " + hp + " healing: " + healing + " sliderMazHP: " + sliderMaxHP);
        //Debug.Log("Slider value: " + (hp + healing) / sliderMaxHP);
        
        if(healing == 0) return 0;

        return (hp + healing)/sliderMaxHP;
    }

    public void ChangeOpacity(float newOpacity)
    {
        hpFill.color = new Color(hpFill.color.r, hpFill.color.g, hpFill.color.b, newOpacity);
    }
}
