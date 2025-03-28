using System;
using UnityEngine;

/// <summary>
/// A float that has upgrades and downgrades.
/// </summary>
// Script by Ruben
[Serializable]
public class UpgradeableFloat
{
    [SerializeField] private float baseValue;

    [SerializeField] private bool stack = true;
    [SerializeField] private LoopBehaviour loopBehaviour = LoopBehaviour.RepeatLast;

    [SerializeField] private Tier[] upgrades;
    [SerializeField] private Tier[] downgrades;

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

    public float GetValue(int tier)
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

        if (!stack)
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

            return array[tier].Modify(baseValue);
        }

        float result = baseValue;

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

            result = array[index].Modify(result);
        }

        return result;
    }

    public UpgradeableFloat(float baseValue, float tierValue, Method method = default, int upgradeAmount = 1, int downgradeAmount = 1)
    {
        this.baseValue = baseValue;

        upgrades = new Tier[upgradeAmount];
        for (int i = 0; i < upgradeAmount; i++)
        {
            upgrades[i] = new Tier(tierValue, method);
        }

        downgrades = new Tier[upgradeAmount];
        for (int i = 0; i < upgradeAmount; i++)
        {
            downgrades[i] = new Tier(tierValue, method == Method.Add ? Method.Subtract : method);
        }
    }

    public UpgradeableFloat(float baseValue, Method method = default, int upgradeAmount = 1, int downgradeAmount = 1)
        : 
        this(baseValue,
        // Goofy ahh syntax
        method switch
        {
            Method.Add => 0,
            Method.Subtract => 0,
            Method.Multiply => 1,
            Method.Divide => 1,
            _ => baseValue,
        }, 
        method, upgradeAmount, downgradeAmount)
    {

    }

    [Serializable]
    private class Tier
    {
        [SerializeField] private float value;
        [SerializeField] private Method method;

        public float Modify(float value)
        {
            switch (method)
            {
                case Method.Add:
                    return value + this.value;

                case Method.Subtract:
                    return value - this.value;

                case Method.Multiply:
                    return value * this.value;

                case Method.Divide:
                    return value / this.value;

                // Override
                default:
                    return this.value;
            }
        }

        public Tier(float value, Method method)
        {
            this.value = value;
            this.method = method;
        }

        public Tier(float value) : this(value, default)
        {

        }
    }

    public enum Method
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        Override,
    }

    public enum LoopBehaviour
    {
        Clamp,
        RepeatLast,
        Reset,
    }
}
