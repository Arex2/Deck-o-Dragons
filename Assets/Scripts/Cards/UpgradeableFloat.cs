using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A float that has upgrades and downgrades.
/// </summary>
[Serializable]
public class UpgradeableFloat
{
    [SerializeField] private float baseValue;

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

        tier = Mathf.Clamp(tier, 0, (isDowngrade ? DowngradeAmount :  UpgradeAmount) - 1);

        return (isDowngrade ? downgrades : upgrades)[tier].Modify(baseValue);
    }

    public UpgradeableFloat(float baseValue, Method method = default, int upgradeAmount = 2, int downgradeAmount = 2)
    {
        this.baseValue = baseValue;

        float tierValue = method switch
        {
            Method.Add => 0,
            Method.Subtract => 0,
            Method.Multiply => 1,
            Method.Divide => 1,
            _ => baseValue,
        };

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
}
