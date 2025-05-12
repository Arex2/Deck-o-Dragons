// Script by Ruben
using System;

/// <summary>
/// Contains the Potency and Duration of a <see cref="StatusEffect"/>. <para/>
/// This data cannot be stored on the <see cref="StatusEffect"/> itself as multiple <see cref="Target"/>s can have the same <see cref="StatusEffect"/>.
/// </summary>
public class StatusEffectData
{
    public StatusEffect StatusEffect => _statusEffect;
    private StatusEffect _statusEffect;

    /// <summary>
    /// How strong the <see cref="StatusEffect"/> is.
    /// </summary>
    public float Potency
    {
        get => _potency;
        set
        {
            if (_potency == value)
            {
                return;
            }

            _potency = value;

            //OnChanged?.Invoke();
        }
    }
    private float _potency;

    /// <summary>
    /// How long the <see cref="StatusEffect"/> lasts.
    /// </summary>
    public int Duration
    {
        get => _duration;
        set
        {
            if (_duration == value)
            {
                return;
            }

            _duration = value;

            //OnChanged?.Invoke();
        }
    }
    private int _duration;

    //public Action OnChanged { get; set; }

    /// <summary>
    /// Custom generic data for a <see cref="StatusEffect"/>.
    /// </summary>
    public object UserData
    {
        get => _userData;
        set
        {
            SetupUserData = true;
            _userData = value;
        }
    }
    private object _userData;

    /// <summary>
    /// Wether or not <see cref="UserData"/> has been setup properly.
    /// </summary>
    public bool SetupUserData { get; private set; } = false;

    public bool Triggered { get; private set; } = false;

    public void Setup(Target target)
    {
        Triggered = false;

        StatusEffect.Setup(target, this);
    }

    public void Trigger()
    {
        Triggered = true;
    }

    /// <summary>
    /// Creates and returns a copy of this <see cref="StatusEffectData"/>.
    /// </summary>
    public StatusEffectData Clone()
    {
        return new(_statusEffect, _potency, _duration);
    }

    public void Merge(StatusEffectData newData)
    {
        float newPotency = newData._potency;
        int newDuration = newData._duration;

        if (newPotency > _potency)
        {
            _potency = newPotency;
        }

        if (newDuration > _duration)
        {
            _duration = newDuration;
        }
    }

    public StatusEffectData(StatusEffect statusEffect, float potency, int duration)
    {
        _statusEffect = statusEffect;
        _potency = potency;
        _duration = duration;
    }
}
