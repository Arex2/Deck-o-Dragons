using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorManager : MonoBehaviour
{
    [SerializeField]
    IncomingDamageIndicator indicatorDMG;
    [SerializeField]
    IncomingHealingIndicator indicatorHEAL;

    [SerializeField]
    GameBehaviour healthSlider;

    float dmgToPlayer = 0;
    float dmgToEnemy = 0;
    float healToPlayer = 0;
    float healToEnemy = 0;

    public void UpdateIndicators(float currentHP, float incomingDMG, float incomingHEAL)
    {
        ClearIndicators();

        //ALTERNATIVT skicka med kort och kolla incomingDMG och incomingHEAL här
        indicatorDMG.ShowIncomingDamage(currentHP,incomingDMG);
        Debug.Log("inc heal: "+ incomingHEAL);
        indicatorHEAL.ShowIncomingHealing(currentHP,incomingHEAL);
    }
    public void UpdateIndicators(Card c) //this doesn't even happen
    {
        float dmg = CalculateDamageToPlayer(c);
        Debug.Log("Damage to player:  " + dmgToPlayer + "  Heal to player: " + healToPlayer + "   health slider hp: " + healthSlider.HP);

        UpdateIndicators(healthSlider.HP, dmgToPlayer, healToPlayer);

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


    private float CalculateDamageToPlayer(Card c)
    {


        if (c == null) return dmgToPlayer;

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

        if(c.GetCardComponent<CardAttack>() != null)
        {
            Debug.Log("Attacking card : total dmg " + c.GetCardComponent<CardAttack>().TotalDamage);
            //c.GetCardComponent<CardAttack>().TargetFilter
            List<Target> targets = TargetManager.GetTargetsWithFilter(Team.Player, c.GetCardComponent<CardAttack>().TargetFilter);
            foreach(Target t in targets)
            {
                Debug.Log("t :" + t);
                if(t.Team == Team.Player) //vet inte om det här behövs ens
                {
                    //updatera player sliders
                    dmgToPlayer = c.GetCardComponent<CardAttack>().TotalDamage;
                }
                
                if(t.Team == Team.Enemy)
                {
                    //updatera enemy sliders
                    dmgToEnemy = c.GetCardComponent<CardAttack>().TotalDamage;
                }
            }
        }

        if (c.GetCardComponent<CardHeal>() != null)
        {
            Debug.Log("Healing card : total dmg " + c.GetCardComponent<CardHeal>().TotalHealing);
            //c.GetCardComponent<CardAttack>().TargetFilter
            List<Target> targets = TargetManager.GetTargetsWithFilter(Team.Player, c.GetCardComponent<CardHeal>().TargetFilter);
            foreach (Target t in targets)
            {
                Debug.Log(t);
                if (t.Team == Team.Player) //vet inte om det här behövs ens
                {
                    //updatera player sliders
                    healToPlayer = c.GetCardComponent<CardHeal>().TotalHealing;
                    Debug.Log("Healing card heal to Player =" + healToPlayer);
                }
                
                if( t.Team == Team.Enemy)
                {
                    //updatera enemy sliders
                    healToEnemy = c.GetCardComponent<CardHeal>().TotalHealing;
                    Debug.Log("Healing card heal to Player =" + healToEnemy);
                }
            }
        }

        /*
        foreach (CardAttack attack in c.GetCardComponents<CardAttack>())
        {
            List<Target> targets = TargetManager.GetTargetsWithFilter(Team.Player, attack.TargetFilter);
        }
        
        if (c.GetCardComponent<CardHeal>() != null)
        {
            Debug.Log("Healing card : total heal " + c.GetCardComponent<CardHeal>().TotalHealing);
        }
        */

        return dmgToPlayer;
    }

    private float CalculateHealingToPlayer()
    {
        return 0;
    }

}
