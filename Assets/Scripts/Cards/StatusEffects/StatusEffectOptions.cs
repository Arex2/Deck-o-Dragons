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
    public StatusEffectData GetData(int level) => new StatusEffectData(GetPotency(level), GetDuration(level));
    public float GetPotency(int level) => potency[level];
    public int GetDuration(int level) => duration[level];

    [SerializeField] private StatusEffect statusEffect;
    [SerializeField] private UpgradeableFloat potency = new(0f, 0f);
    [SerializeField] private UpgradeableInt duration = new(0, 0);
}
