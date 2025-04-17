using System;
using UnityEngine;

[Serializable]
public class StatusEffectOptions
{
    public StatusEffect StatusEffect => statusEffect;
    public StatusEffectData GetData(int tier) => new StatusEffectData(potency[tier], duration[tier]);

    [SerializeField] private StatusEffect statusEffect;
    [SerializeField] private UpgradeableFloat potency = new(0f, 0f);
    [SerializeField] private UpgradeableInt duration = new(0, 0);
}
