using System;
using UnityEngine;

/// <summary>
/// The base script for any Upgradeable variable such as <see cref="UpgradeableInt"/> or <see cref="UpgradeableFloat"/>. <para/>
/// Also used by the general purpose <see cref="Upgradeable{T}"/> class.
/// </summary>
// Script by Ruben
public abstract class UpgradeableBase<T, Level>
{
    public abstract bool CanStack { get; }

    [SerializeField] protected T baseValue;

    [SerializeField] protected bool stack = true;
    [SerializeField] protected LoopBehaviour loopBehaviour = LoopBehaviour.RepeatLast;

    [SerializeField] protected Level[] upgrades;
    [SerializeField] protected Level[] downgrades;

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

    public T this[int level] => GetValue(level);

    public T GetValue(int level)
    {
        if (level == 0)
        {
            return ModifyGetValueResult(baseValue);
        }

        bool isDowngrade = level < 0;

        if (isDowngrade)
        {
            level = Mathf.Abs(level);
        }

        level -= 1;

        Level[] array = isDowngrade ? downgrades : upgrades;
        int length = isDowngrade ? DowngradeAmount : UpgradeAmount;

        if (!stack || !CanStack)
        {
            switch (loopBehaviour)
            {
                default:
                    level = Mathf.Clamp(level, 0, length - 1);
                    break;

                case LoopBehaviour.Reset:
                    if (length != 0)
                    {
                        level %= length;
                    }
                    break;
            }

            return ModifyGetValueResult(GetLevelValue(array[level], baseValue));
        }

        T result = baseValue;

        int limit = level;

        if (loopBehaviour == LoopBehaviour.Clamp)
        {
            limit = Mathf.Min(level, length - 1);
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
                        if (length != 0)
                        {
                            level %= length;
                        }
                        break;
                }
            }

            result = GetLevelValue(array[index], result);
        }

        return ModifyGetValueResult(result);
    }

    public virtual T ModifyGetValueResult(T result) => result;

    protected abstract T GetLevelValue(Level level, T currentValue);

    public string ToString(int level)
    {
        return GetValue(level).ToString();
    }

    public UpgradeableBase(T baseValue, int upgradeAmount, int downgradeAmount, Func<int, Level> forEachUpgrade, Func<int, Level> forEachDowngrade)
    {
        this.baseValue = baseValue;
        stack = CanStack;

        upgrades = new Level[upgradeAmount];
        if (forEachUpgrade != null)
        {
            for (int i = 0; i < upgradeAmount; i++)
            {
                upgrades[i] = forEachUpgrade.Invoke(i);
            }
        }

        downgrades = new Level[upgradeAmount];
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
