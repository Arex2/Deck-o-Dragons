using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveStatusEffect : CardComponent, IUseSingle
{
    public override TargetFilter TargetFilter => targetFilter;

    [SerializeField] private TargetFilter targetFilter = new(TargetFilter.FilterTeam.Opponent, TargetFilter.FilterMode.Leader);

    [Space]
    [SerializeField] private StatusEffectOptions statusEffect;

    public void Use(Target target)
    {
        target.ApplyStatusEffect(statusEffect.StatusEffect, statusEffect.GetData(Tier));
    }
}
