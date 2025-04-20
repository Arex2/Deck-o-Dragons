using System;
using UnityEngine;

/// <summary>
/// The base class for any number that has upgrades and downgrades. <para/>
/// Used by <see cref="UpgradeableInt"/> and <see cref="UpgradeableFloat"/>.
/// </summary>
// Script by Ruben
[Serializable]
public abstract class UpgradeableNumber<T> : UpgradeableBase<T, UpgradeableNumber<T>.NumberTier>
{
    public override bool CanStack => true;

    protected abstract T AddMethod(T a, T b);

    protected abstract T SubtractMethod(T a, T b);

    protected abstract T MultiplyMethod(T a, T b);

    protected abstract T DivideMethod(T a, T b);

    protected override T GetTierValue(NumberTier tier, T currentValue)
    {
        return tier.GetValue(this, currentValue);
    }

    public UpgradeableNumber(T baseValue, T tierValue, Method method = default, int upgradeAmount = 1, int downgradeAmount = 1)
        :
        base(baseValue, upgradeAmount, downgradeAmount,
            (index) => new NumberTier(tierValue, method),
            (index) => new NumberTier(tierValue, method == Method.Add ? Method.Subtract : method))
    {

    }

    public UpgradeableNumber(T baseValue, Method method = default, int upgradeAmount = 1, int downgradeAmount = 1)
        :
        this(baseValue, default, method, upgradeAmount, downgradeAmount)
    {

    }

    public enum Method
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        Override,
    }

    [Serializable]
    public class NumberTier
    {
        [SerializeField] private T value;
        [SerializeField] private Method method;

        public T GetValue(UpgradeableNumber<T> upgradeable, T value)
        {
            switch (method)
            {
                case Method.Add:
                    return upgradeable.AddMethod(value, this.value);

                case Method.Subtract:
                    return upgradeable.SubtractMethod(value,  this.value);

                case Method.Multiply:
                    return upgradeable.MultiplyMethod(value, this.value);

                case Method.Divide:
                    return upgradeable.DivideMethod(value, this.value);

                // Override
                default:
                    return this.value;
            }
        }

        public NumberTier(T value, Method method)
        {
            this.value = value;
            this.method = method;
        }

        public NumberTier(T value) : this(value, default)
        {

        }
    }
}
