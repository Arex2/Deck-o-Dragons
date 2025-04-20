using UnityEngine;

/// <summary>
/// <see cref="CardComponent"/> that will change the players mana stat.
/// </summary>
// Script by Ruben
public class ModifyMana : CardComponent, IUse
{
    [SerializeField] private UpgradeableFloat manaAmount = new(1);

    public void Use()
    {
        // TODO: THIS IS SUPER TEMPORARY AND WE SHOULD PROBABLY HAVE A BETTER MANA SYSTEM
        FindObjectOfType<GameBehaviour>().LoseMana(-Mathf.RoundToInt(manaAmount.GetValue(Tier)));
    }

    [ReplaceDescriptionKeyword]
    [ReplaceDescriptionKeyword("MANA")]
    private string ReplaceManaKeyword()
    {
        return manaAmount.ToString(Tier);
    }
}
