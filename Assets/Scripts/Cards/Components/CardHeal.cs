using UnityEngine;

/// <summary>
/// A <see cref="CardComponent"/> responsible for healing a <see cref="Target"/>.
/// </summary>
// Script by Ruben
[AddComponentMenu("Card Heal")]
public class CardHeal : CardComponent, IUseSingle
{
    public override TargetFilter TargetFilter => targetFilter;
    [SerializeField] private TargetFilter targetFilter = new(TargetFilter.FilterTeam.Own, TargetFilter.FilterMode.Leader);

    [Space]
    [SerializeField] private UpgradeableFloat healing = new(2);

    public void Use(Target target)
    {
        target.Heal(healing.GetValue(Level));
    }

    [ReplaceDescriptionKeyword]
    [ReplaceDescriptionKeyword("HEALING")]
    private string ReplaceDescriptionKeyword()
    {
        return healing.ToString(Level);
    }
}
