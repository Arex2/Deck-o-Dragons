using System;
using UnityEngine;

/// <summary>
/// A generic class that has upgrades and downgrades for the given <typeparamref name="T"/> type.
/// </summary>
// Script by Ruben
[Serializable]
public class Upgradeable<T> : UpgradeableBase<T, Upgradeable<T>.Level>
{
    public override bool CanStack => false;

    protected override T GetLevelValue(Level level, T currentValue)
    {
        return level.GetValue(currentValue);
    }

    public Upgradeable(T baseValue, int upgradeAmount, int downgradeAmount, Func<int, Level> forEachUpgrade, Func<int, Level> forEachDowngrade) : base(baseValue, upgradeAmount, downgradeAmount, forEachUpgrade, forEachDowngrade)
    {
    }

    public Upgradeable(T baseValue, int upgradeAmount = 1, int downgradeAmount = 1) : base(baseValue, upgradeAmount, downgradeAmount)
    {
    }

    [Serializable]
    public class Level
    {
        [SerializeField] private T value;

        public Level(T value)
        {
            this.value = value;
        }

        public T GetValue(T value)
        {
            return this.value;
        }
    }
}
