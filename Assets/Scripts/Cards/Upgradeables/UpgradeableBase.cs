using System;
using UnityEngine;

/// <summary>
/// The base script for any Upgradeable variable such as <see cref="UpgradeableInt"/> or <see cref="UpgradeableFloat"/>. <para/>
/// Also used by the general purpose <see cref="Upgradeable{T}"/> class.
/// </summary>
// Script by Ruben
public abstract class UpgradeableBase<T, Tier>
{
    public abstract bool CanStack { get; }

    [SerializeField] protected T baseValue;

    [SerializeField] protected bool stack = true;
    [SerializeField] protected LoopBehaviour loopBehaviour = LoopBehaviour.RepeatLast;

    [SerializeField] protected Tier[] upgrades;
    [SerializeField] protected Tier[] downgrades;

    public int UpgradeAmount
    {
        get
        {
            if (!_upgradeAmount.HasValue)
            {
                _upgradeAmount = upgrades.Length;
            }

            return _upgradeAmount.Value;
        }
    }
    private int? _upgradeAmount;

    public int DowngradeAmount
    {
        get
        {
            if (!_downgradesLength.HasValue)
            {
                _downgradesLength = upgrades.Length;
            }

            return _downgradesLength.Value;
        }
    }
    private int? _downgradesLength;

    public T GetValue(int tier)
    {
        if (tier == 0)
        {
            return baseValue;
        }

        bool isDowngrade = tier < 0;

        if (isDowngrade)
        {
            tier = Mathf.Abs(tier);
        }

        tier -= 1;

        Tier[] array = isDowngrade ? downgrades : upgrades;
        int length = isDowngrade ? DowngradeAmount : UpgradeAmount;

        if (!stack || !CanStack)
        {
            switch (loopBehaviour)
            {
                default:
                    tier = Mathf.Clamp(tier, 0, length - 1);
                    break;

                case LoopBehaviour.Reset:
                    tier %= length;
                    break;
            }

            return GetTierValue(array[tier], baseValue);
        }

        T result = baseValue;

        int limit = tier;

        if (loopBehaviour == LoopBehaviour.Clamp)
        {
            limit = Mathf.Min(tier, length - 1);
        }

        for (int i = 0; i <= limit; i++)
        {
            int index = i;

            if (index > length - 1)
            {
                switch (loopBehaviour)
                {
                    default:
                        index = length - 1;
                        break;

                    case LoopBehaviour.Reset:
                        index %= length;
                        break;
                }
            }

            result = GetTierValue(array[index], result);
        }

        return result;
    }

    protected abstract T GetTierValue(Tier tier, T currentValue);

    public string ToString(int tier)
    {
        return GetValue(tier).ToString();
    }

    public UpgradeableBase(T baseValue, int upgradeAmount, int downgradeAmount, Func<int, Tier> forEachUpgrade, Func<int, Tier> forEachDowngrade)
    {
        this.baseValue = baseValue;
        stack = CanStack;

        upgrades = new Tier[upgradeAmount];
        if (forEachUpgrade != null)
        {
            for (int i = 0; i < upgradeAmount; i++)
            {
                upgrades[i] = forEachUpgrade.Invoke(i);
            }
        }

        downgrades = new Tier[upgradeAmount];
        if (forEachDowngrade != null)
        {
            for (int i = 0; i < upgradeAmount; i++)
            {
                downgrades[i] = forEachDowngrade.Invoke(i);
            }
        }
    }

    public UpgradeableBase(T baseValue, int upgradeAmount = 1, int downgradeAmount = 1) : this(baseValue, upgradeAmount, downgradeAmount, null, null)
    {

    }

    public enum LoopBehaviour
    {
        Clamp,
        RepeatLast,
        Reset,
    }
}
