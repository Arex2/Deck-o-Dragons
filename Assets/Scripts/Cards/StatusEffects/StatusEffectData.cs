using System;
using UnityEngine;

[Serializable]
public class StatusEffectData
{
    public float Potency
    {
        get => potency;
        set => potency = value;
    }

    public int Duration
    {
        get => duration;
        set => duration = value;
    }

    [SerializeField] private float potency;
    [SerializeField] private int duration;

    public StatusEffectData(float potency, int duration)
    {
        this.potency = potency;
        this.duration = duration;
    }
}
