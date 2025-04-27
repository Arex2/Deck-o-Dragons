using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorManager : MonoBehaviour
{
    [SerializeField]
    IncomingDamageIndicator indicatorDMG;
    [SerializeField]
    IncomingHealingIndicator indicatorHEAL;

    public void UpdateIndicators(float currentHP, float incomingDMG, float incomingHEAL)
    {
        //ALTERNATIVT skicka med kort och kolla incomingDMG och incomingHEAL här

        indicatorDMG.ShowIncomingDamage(currentHP,incomingDMG);
        indicatorHEAL.ShowIncomingHealing(currentHP,incomingHEAL);
    }
    public void UpdateIndicators(Card c)
    {
        UpdateIndicators(c);
    }

    public void ClearIndicators()
    {
        indicatorDMG.hide();
        indicatorHEAL.hide();
    }

}
