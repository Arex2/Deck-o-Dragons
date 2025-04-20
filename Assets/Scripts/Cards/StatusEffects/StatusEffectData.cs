using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the Potency and Duration of a <see cref="StatusEffect"/>. <para/>
/// This data cannot be stored on the <see cref="StatusEffect"/> itself as multiple <see cref="Target"/>s can have the same <see cref="StatusEffect"/>.
/// </summary>
// Script by Ruben
public class StatusEffectData
{
    /// <summary>
    /// How strong the <see cref="StatusEffect"/> is.
    /// </summary>
    public float Potency
    {
        get => potency;
        set
        {
            if (potency == value)
            {
                return;
            }

            potency = value;

            //OnChanged?.Invoke();
        }
    }

    /// <summary>
    /// How long the <see cref="StatusEffect"/> lasts.
    /// </summary>
    public int Duration
    {
        get => duration;
        set
        {
            if (duration == value)
            {
                return;
            }

            duration = value;

            //OnChanged?.Invoke();
        }
    }

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

    [SerializeField] private float potency;
    [SerializeField] private int duration;

    /// <summary>
    /// Creates and returns a copy of this <see cref="StatusEffectData"/>.
    /// </summary>
    public StatusEffectData Clone()
    {
        return new StatusEffectData(potency, duration);
    }

    public StatusEffectData(float potency, int duration)
    {
        this.potency = potency;
        this.duration = duration;
    }
}
