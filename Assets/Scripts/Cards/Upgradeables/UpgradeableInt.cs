using System;

/// <summary>
/// An <see cref="int"/> that has upgrades and downgrades.
/// </summary>
// Script by Ruben
[Serializable]
public class UpgradeableInt : UpgradeableNumber<int>
{
    public override int AddMethod(int a, int b)
    {
        return a + b;
    }

    public override int SubtractMethod(int a, int b)
    {
        return a - b;
    }

    public override int MultiplyMethod(int a, int b)
    {
        return a * b;
    }

    public override int DivideMethod(int a, int b)
    {
        return a / b;
    }

    public UpgradeableInt(int baseValue, int tierValue, Method method = default, int upgradeAmount = 1, int downgradeAmount = 1)
        :
        base(baseValue, tierValue, method, upgradeAmount, downgradeAmount)
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
