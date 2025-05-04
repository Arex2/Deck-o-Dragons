using System;
using UnityEngine;

/// <summary>
/// An <see cref="int"/> that has upgrades and downgrades.
/// </summary>
// Script by Ruben
[Serializable]
public class UpgradeableInt : UpgradeableNumber<int>
{
    protected override int AddMethod(int a, int b)
    {
        return a + b;
    }

    protected override int SubtractMethod(int a, int b)
    {
        return a - b;
    }

    protected override int MultiplyMethod(int a, int b)
    {
        return a * b;
    }

    protected override int DivideMethod(int a, int b)
    {
        if (b == 0)
        {
            Debug.LogWarning("Diving by zero!");
            return 0;
        }

        return a / b;
    }

    public override int ModifyGetValueResult(int result)
    {
        bool minLimitEnabled = minLimit.Enabled;
        bool maxLimitEnabled = maxLimit.Enabled;

        if (minLimitEnabled && maxLimitEnabled)
        {
            result = Mathf.Clamp(result, minLimit.Value, maxLimit.Value);
        }
        else if (minLimitEnabled && !maxLimitEnabled)
        {
            result = Mathf.Max(result, minLimit.Value);
        }
        else if (!minLimitEnabled && maxLimitEnabled)
        {
            result = Mathf.Min(result, maxLimit.Value);
        }

        return result;
    }

    public UpgradeableInt(int baseValue, int levelValue, Method method = default, int upgradeAmount = 1, int downgradeAmount = 1)
        :
        base(baseValue, levelValue, method, upgradeAmount, downgradeAmount)
    {

    }

    public UpgradeableInt(int baseValue, Method method = default, int upgradeAmount = 1, int downgradeAmount = 1)
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
}
