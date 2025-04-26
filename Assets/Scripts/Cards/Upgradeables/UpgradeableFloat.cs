using System;
using UnityEngine;

/// <summary>
/// A <see cref="float"/> that has upgrades and downgrades.
/// </summary>
// Script by Ruben
[Serializable]
public class UpgradeableFloat : UpgradeableNumber<float>
{
    protected override float AddMethod(float a, float b)
    {
        return a + b;
    }

    protected override float SubtractMethod(float a, float b)
    {
        return a - b;
    }

    protected override float MultiplyMethod(float a, float b)
    {
        return a * b;
    }

    protected override float DivideMethod(float a, float b)
    {
        if (b == 0)
        {
            Debug.LogWarning("Diving by zero!");
            return 0;
        }

        return a / b;
    }

    public override float ModifyGetValueResult(float result)
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

    public UpgradeableFloat(float baseValue, float levelValue, Method method = default, int upgradeAmount = 1, int downgradeAmount = 1)
        :
        base(baseValue, levelValue, method, upgradeAmount, downgradeAmount)
    {

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
}
