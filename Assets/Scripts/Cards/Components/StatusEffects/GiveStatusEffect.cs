using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <see cref="CardComponent"/> that gives a single <see cref="StatusEffect"/> to a <see cref="Target"/>.
/// </summary>
// Script by Ruben
[AddComponentMenu("Status Effects/Give Status Effects")]
public class GiveStatusEffect : CardComponent
{
    public override TargetFilter TargetFilter => targetFilter;

    [SerializeField] private TargetFilter targetFilter = new(TargetFilter.FilterTeam.Opponent, TargetFilter.FilterMode.Leader);

    [Space]
    [SerializeField] private StatusEffectOptions statusEffect;


    public override void Play(List<Target> targets)
    {
        foreach (Target target in targets)
        {
            target.AddStatusEffect(statusEffect.GetData(Level));
        }
    }

    [ReplaceDescriptionKeyword("STATUS_EFFECT_NAME")]
    private string ReplaceNameKeyword()
    {
        return statusEffect.StatusEffect.DisplayName;
    }

    [ReplaceDescriptionKeyword("STATUS_EFFECT_DESCRIPTION")]
    private string ReplaceDescriptionKeyword()
    {
        return statusEffect.StatusEffect.GetDescription(statusEffect.GetData(Level));
    }

    [ReplaceDescriptionKeyword("DURATION")]
    private string ReplaceDurationKeyword()
    {
        return UpgradeablesManager.ColorBasedOnLevel(statusEffect.GetDuration(Level).ToString(), Level);
    }

    [ReplaceDescriptionKeyword("POTENCY")]
    private string ReplacePotencyKeyword()
    {
        return UpgradeablesManager.ColorBasedOnLevel(StatusEffect.GetPotencyString(statusEffect.GetPotency(Level), statusEffect.StatusEffect.PotencyIsPercent), Level);
    }
}
