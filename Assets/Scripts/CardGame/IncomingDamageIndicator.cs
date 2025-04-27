using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IncomingDamageIndicator : MonoBehaviour
{
    [SerializeField]
    private Slider damageSlider;
    [SerializeField]
    private Slider hpSlider;

    [SerializeField]
    float currentHp;
    [SerializeField]
    float incomingDMG;
    public void Test()
    {
        ShowIncomingDamage(incomingDMG, hpSlider.value *10);
    }

    public void ShowIncomingDamage(float incomingDamage, float currentHP)
    {
        damageSlider.value = CalculateDamageToSlider(incomingDamage, currentHP);
    }

    public void hide()
    {
        damageSlider.value = 0;
    }

    private float CalculateDamageToSlider(float damage, float hp)
    {
        //damage to make  /  current hp  =  slider value
        return damage/hp;
    }
}
