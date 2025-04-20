using UnityEngine;

/// <summary>
/// <see cref="CardComponent"/> that removes all status effects in an array from a <see cref="Target"/>.
/// </summary>
// Script by Ruben
[AddComponentMenu("Status Effects/Remove Status Effects")]
public class RemoveStatusEffects : CardComponent, IUseSingle
{
    public override TargetFilter TargetFilter => targetFilter;

    [SerializeField] private TargetFilter targetFilter = new(TargetFilter.FilterTeam.Opponent, TargetFilter.FilterMode.Leader);

    [Space]
    [SerializeField] private StatusEffect[] statusEffects;

    [Space]
    [SerializeField] private bool removeAllDebuffs;
    [SerializeField] private bool removeAllNonDebuffs;

    public void Use(Target target)
    {
        if (removeAllDebuffs && removeAllNonDebuffs)
        {
            target.ClearAllStatusEffects();
            return;
        }
        else if (removeAllDebuffs && !removeAllNonDebuffs)
        {
            target.ClearAllDebuffs();
        }
        else if (!removeAllDebuffs && removeAllNonDebuffs)
        {
            target.ClearAllNonDebuffs();
        }

        foreach (StatusEffect statusEffect in statusEffects)
        {
            target.RemoveStatusEffect(statusEffect);
        }
    }
}
