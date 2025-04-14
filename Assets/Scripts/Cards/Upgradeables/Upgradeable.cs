using System;
using UnityEngine;

/// <summary>
/// A generic class that has upgrades and downgrades for the given <typeparamref name="T"/> type.
/// </summary>
// Script by Ruben
[Serializable]
public class Upgradeable<T> : UpgradeableBase<T, Upgradeable<T>.Tier>
{
    public override bool CanStack => false;

    protected override T GetTierValue(Tier tier, T currentValue)
    {
        return tier.GetValue(currentValue);
    }

    public Upgradeable(T baseValue, int upgradeAmount, int downgradeAmount, Func<int, Tier> forEachUpgrade, Func<int, Tier> forEachDowngrade) : base(baseValue, upgradeAmount, downgradeAmount, forEachUpgrade, forEachDowngrade)
    {
    }

    public Upgradeable(T baseValue, int upgradeAmount = 1, int downgradeAmount = 1) : base(baseValue, upgradeAmount, downgradeAmount)
    {
    }

    [Serializable]
    public class Tier
    {
        [SerializeField] private T value;

        public Tier(T value)
        {
            this.value = value;
        }

        public T GetValue(T value)
        {
            return this.value;
        }
    }
}
