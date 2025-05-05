using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IndicatorManager : MonoBehaviour
{
    [SerializeField]
    IncomingDamageIndicator indicatorDMG;
    [SerializeField]
    IncomingHealingIndicator indicatorHEAL;
    [SerializeField]
    IncomingDamageIndicator enemyIndicatorDMG;
    [SerializeField]
    IncomingHealingIndicator enemyIndicatorHEAL;

    [SerializeField]
    GameBehaviour healthSlider;
    [SerializeField]
    Slider enemyHealthSlider;

    float dmgToPlayer = 0;
    float dmgToEnemy = 0;
    float healToPlayer = 0;
    float healToEnemy = 0;

    /// <summary>
    /// Used to update indicators in cases when the calculations
    /// were made before the health had been updated.
    /// Fixes situations like: 
    /// a damaging card is played, 
    /// a healing card is selected while damage is being applied,
    /// and healing is calculated on the wrong current health.
    /// </summary>
    public void UpdateIndicatorsForOldCard()
    {
        //Debug.Log("Update Indicators for OLD card.");
        UpdateIndicatorsPlayer(healthSlider.HP, dmgToPlayer, healToPlayer, healthSlider.MaxHP);
        UpdateIndicatorsEnemy(enemyHealthSlider.value, dmgToEnemy, healToEnemy, enemyHealthSlider.maxValue);
    }

    public void UpdateIndicators(Card c)
    {
        //Debug.Log("Update Indicators now: " + c);
        ClearIndicators();
        ReadCard(c);
        //Debug.Log("Damage to player:  " + dmgToPlayer + "  Heal to player: " + healToPlayer + "   health slider hp: " + healthSlider.HP);
        //Debug.Log("Damage to enemy:  " + dmgToEnemy + "  Heal to enemy: " + healToEnemy + "   health slider hp: " + enemyHealthSlider.value );
        //Debug.Log("player hp slider: " + healthSlider.HP + "  enemy health slider: " + enemyHealthSlider.value);
        UpdateIndicatorsPlayer(healthSlider.HP, dmgToPlayer, healToPlayer, healthSlider.MaxHP);
        UpdateIndicatorsEnemy(enemyHealthSlider.value, dmgToEnemy, healToEnemy, enemyHealthSlider.maxValue);
    }

    public void UpdateIndicatorsPlayer(float currentHP, float incomingDMG, float incomingHEAL, float maxHP)
    {
        indicatorDMG.ShowIncomingDamage(currentHP,incomingDMG);
        indicatorHEAL.ShowIncomingHealing(currentHP,incomingHEAL, maxHP);
    }
    public void UpdateIndicatorsEnemy(float currentHP, float incomingDMG, float incomingHEAL, float maxHP)
    {
        //behöver få enemy healthbar oavsätt vilken enemy

        enemyIndicatorDMG.ShowIncomingDamage(currentHP, incomingDMG);
        enemyIndicatorHEAL.ShowIncomingHealing(currentHP, incomingHEAL, maxHP);
    }


    public void ClearIndicators()
    {
        //reset values
        dmgToPlayer = 0;
        dmgToEnemy = 0;

        healToPlayer = 0;
        healToEnemy = 0;


        indicatorDMG.hide();
        indicatorHEAL.hide();

        enemyIndicatorDMG.hide();
        enemyIndicatorHEAL.hide();
    }

    //Methods read damage and healing values from the card,
    //and assigns this to variables
    private void ReadCard(Card c)
    {
        ReadHealing(c);
        ReadDamage(c);
    }

    private void ReadDamage(Card c)
    {
        if (c == null) return;

        /*
        //RUBENS KOD:
        Dictionary<Target, float> totalDamageToTargets = new();

        Debug.Log(c);

        if (c.GetCardComponents<CardAttack>() != null)
        {
            foreach (CardAttack attack in c.GetCardComponents<CardAttack>())
            {
                float damage = attack.TotalDamage;

                List<Target> targets = TargetManager.GetTargetsWithFilter(Team.Player, attack.TargetFilter);

                foreach (Target target in targets)
                {
                    //Tally up total damage to each target
                    if (totalDamageToTargets.ContainsKey(target))
                    {
                        totalDamageToTargets[target] += damage;
                    }
                    else
                    {
                        totalDamageToTargets.Add(target, damage);
                    }
                }
                dmgToPlayer = damage;
            }

            foreach (KeyValuePair<Target, float> keyValuePair in totalDamageToTargets)
            {
                Debug.Log(keyValuePair.Key.name + " will take a total of " + keyValuePair.Value + " damage from this card.");
            }

        }
        Debug.Log("2Damage to player:  " + dmgToPlayer);

        foreach (float damage in totalDamageToTargets.Values)
        {
            dmgToPlayer += damage;
        }
        */

        if (!c.TryGetCardComponents(out CardAttack[] components)) return;

        foreach (CardAttack attack in components)
        {
            switch (attack.TargetFilter.Team)
            {
                //mot båda
                case TargetFilter.FilterTeam.Chosen:
                case TargetFilter.FilterTeam.Random:
                case TargetFilter.FilterTeam.All:
                    dmgToPlayer += attack.TotalDamage;
                    dmgToEnemy = attack.TotalDamage;
                    break;

                //mot player
                case TargetFilter.FilterTeam.Own:
                    dmgToPlayer += attack.TotalDamage;
                    break;

                //mot enemy
                case TargetFilter.FilterTeam.Opponent:
                    dmgToEnemy = attack.TotalDamage;
                    break;
            }
        }
    }

    private void ReadHealing(Card c)
    {
        if (c == null) return;

        if (!c.TryGetCardComponents(out CardHeal[] components)) return;

        foreach (CardHeal heal in components)
        {
            switch (heal.TargetFilter.Team)
            {
                //mot båda
                case TargetFilter.FilterTeam.Chosen:
                case TargetFilter.FilterTeam.Random:
                case TargetFilter.FilterTeam.All:
                    healToPlayer += heal.TotalHealing;
                    healToEnemy += heal.TotalHealing;
                    break;

                //mot player
                case TargetFilter.FilterTeam.Own:
                    healToPlayer += heal.TotalHealing;
                    break;

                //mot enemy
                case TargetFilter.FilterTeam.Opponent:
                    healToEnemy += heal.TotalHealing;
                    break;
            }
        }
    }

}
