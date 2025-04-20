using System;
using UnityEngine;

/// <summary>
/// Class meant to be used as options for how a status effect should be applied to a <see cref="Target"/>. <para/>
/// Provides Upgradeable fields for the Potency and Duration of the status effect.
/// </summary>
// Script by Ruben
[Serializable]
public class StatusEffectOptions
{
    public StatusEffect StatusEffect => statusEffect;
    public StatusEffectData GetData(int tier) => new StatusEffectData(GetPotency(tier), GetDuration(tier));
    public float GetPotency(int tier) => potency[tier];
    public int GetDuration(int tier) => duration[tier];

    [SerializeField] private StatusEffect statusEffect;
    [SerializeField] private UpgradeableFloat potency = new(0f, 0f);
    [SerializeField] private UpgradeableInt duration = new(0, 0);
}
