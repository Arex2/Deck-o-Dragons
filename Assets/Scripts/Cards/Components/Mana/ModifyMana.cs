using UnityEngine;

/// <summary>
/// <see cref="CardComponent"/> that will change the players mana stat.
/// </summary>
// Script by Ruben
[AddComponentMenu("Modify Mana")]
public class ModifyMana : CardComponent, IUse
{
    [SerializeField] private UpgradeableInt manaAmount = new(1);

    private GameBehaviour _gameBehaviour;

    public void Use()
    {
        if (_gameBehaviour == null)
        {
            _gameBehaviour = GameBehaviour.Instance;

            if (_gameBehaviour == null)
            {
                Debug.LogWarning($"There is no {nameof(GameBehaviour)} in the scene!");
                return;
            }
        }

        int value = manaAmount.GetValue(Level);

        if (value > 0)
        {
            _gameBehaviour.GainMana(value);
        }
        else if (value < 0)
        {
            _gameBehaviour.LoseMana(value);
        }
    }

    [ReplaceDescriptionKeyword]
    [ReplaceDescriptionKeyword("MANA")]
    private string ReplaceManaKeyword()
    {
        return manaAmount.ToString(Level);
    }
}
