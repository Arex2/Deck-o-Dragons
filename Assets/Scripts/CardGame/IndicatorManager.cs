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


    public void UpdateIndicators(Card c)
    {
        ClearIndicators();
        ReadCard(c);
        Debug.Log("Damage to player:  " + dmgToPlayer + "  Heal to player: " + healToPlayer + "   health slider hp: " + healthSlider.HP);
        Debug.Log("Damage to enemy:  " + dmgToEnemy + "  Heal to enemy: " + healToEnemy + "   health slider hp: " + healthSlider.HP);
        UpdateIndicatorsPlayer(healthSlider.HP, dmgToPlayer, healToPlayer);
        //UpdateIndicatorsEnemy(enemyHealthSlider.value, dmgToEnemy, healToEnemy);
    }

    public void UpdateIndicatorsPlayer(float currentHP, float incomingDMG, float incomingHEAL)
    {
        indicatorDMG.ShowIncomingDamage(currentHP,incomingDMG);
        indicatorHEAL.ShowIncomingHealing(currentHP,incomingHEAL);
    }
    public void UpdateIndicatorsEnemy(float currentHP, float incomingDMG, float incomingHEAL)
    {
        //behöver få enemy healthbar oavsätt vilken enemy

        //indicatorDMG.ShowIncomingDamage(currentHP, incomingDMG);
        //indicatorHEAL.ShowIncomingHealing(currentHP, incomingHEAL);
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

        if (c.GetCardComponent<CardAttack>() != null)
        {

            foreach(CardAttack attack in c.GetCardComponents<CardAttack>())
            {
                //mot player
                if (attack.TargetFilter.Team == TargetFilter.FilterTeam.Own)
                {
                    dmgToPlayer += attack.TotalDamage;
                }
                //mot enemy
                if(attack.TargetFilter.Team == TargetFilter.FilterTeam.Opponent)
                {
                    dmgToEnemy = c.GetCardComponent<CardAttack>().TotalDamage;
                }
            }
        }

    }

    private void ReadHealing(Card c)
    {
        if (c == null) return;

        if (c.GetCardComponent<CardHeal>() != null)
        {
            foreach (CardHeal heal in c.GetCardComponents<CardHeal>())
            {
                if (heal.TargetFilter.Team == TargetFilter.FilterTeam.Own)
                {
                    healToPlayer += heal.TotalHealing;
                }

                if (heal.TargetFilter.Team == TargetFilter.FilterTeam.Opponent)
                {
                    healToEnemy += heal.TotalHealing;
                }
            }
        }

    }

}
