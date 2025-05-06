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

    public void ShowIncomingDamage(float currentHP, float incomingDamage)
    {
        damageSlider.normalizedValue = CalculateDamageToSlider(incomingDamage, currentHP);
    }

    public void Hide()
    {
        damageSlider.normalizedValue = 0;
    }

    private float CalculateDamageToSlider(float damage, float hp)
    {
        //damage to make  /  current hp  =  slider value
        if (hp == 0) return 0;

        return damage/hp;
    }
}
