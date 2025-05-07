using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A <see cref="CardComponent"/> responsible for healing a <see cref="Target"/>.
/// </summary>
// Script by Ruben
[AddComponentMenu("Card Heal")]
public class CardHeal : CardComponent
{
    /// <summary>
    /// The total healing this Component will give to a <see cref="Target"/>. <para/>
    /// Takes into account <see cref="Healing"/> and <see cref="HealingMultiplier"/>.
    /// </summary>
    public float TotalHealing => Healing * HealingMultiplier;

    /// <summary>
    /// The name of the key used to set the <see cref="HealingMultiplier"/> in the <see cref="CardData"/>.
    /// </summary>
    public const string HEALING_MULTIPLIER_KEY_NAME = "HEALING_MULTIPLIER";

    /// <summary>
    /// All healing is multiplied by this value.
    /// </summary>
    public float HealingMultiplier => GetCardData<float>(HEALING_MULTIPLIER_KEY_NAME, 1);

    /// <summary>
    /// How much healing this component will give, does not account for <see cref="HealingMultiplier"/>.
    /// </summary>
    public float Healing => healing[Level];

    public override TargetFilter TargetFilter => targetFilter;
    [SerializeField] private TargetFilter targetFilter = new(TargetFilter.FilterTeam.Own, TargetFilter.FilterMode.Leader);

    [Space]
    [SerializeField] private UpgradeableFloat healing = new(20, 10);

    [Space]
    [SerializeField] private CardVFXReference vfx = new("Heal");

    public override void Play(List<Target> targets)
    {
        foreach (Target target in targets)
        {
            bool addedActionToVFX = false;

            // VFX
            foreach (CardVFX vfx in SpawnVFX(vfx))
            {
                if (vfx == null)
                {
                    continue;
                }

                vfx.SetTarget(target);

                if (vfx.TriggersActions && !addedActionToVFX)
                {
                    vfx.AddAction((cardVFX) => target.Heal(new(TotalHealing)));
                    addedActionToVFX = true;
                }
            }

            if (!addedActionToVFX)
            {
                target.Heal(new(TotalHealing));
            }
        }
    }

    [ReplaceDescriptionKeyword]
    [ReplaceDescriptionKeyword("HEALING")]
    private string ReplaceDescriptionKeyword()
    {
        return UpgradeablesManager.ColorBasedOnLevel(Mathf.Round(Healing).ToString(), Level);
    }
}
