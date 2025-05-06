using System;
using System.Collections.Generic;
using UnityEngine;

public class TargetEffects : MonoBehaviour, ITargetCallbacks
{
    [SerializeField] private StatusEffectVisual[] statusEffectVisuals;

    private Target _target;

    private void Awake()
    {
        _target = GetComponentInParent<Target>(true);
    }

    public void OnHurt()
    {
        TriggerStatusEffectVisuals();
    }

    public void OnDeath()
    {

    }

    public void OnHeal()
    {
        TriggerStatusEffectVisuals();
    }

    public void OnUpdateHP()
    {
        TriggerStatusEffectVisuals();
    }

    public void OnUpdateStatusEffects()
    {
        foreach (StatusEffectVisual visual in statusEffectVisuals)
        {
            bool enabled = _target.StatusEffectsData.ContainsKey(visual.Effect);
            visual.Toggle(enabled);
        }
    }

    public void OnTurnStart()
    {
        TriggerStatusEffectVisuals();
    }

    public void OnTurnEnd()
    {
        TriggerStatusEffectVisuals();
    }

    private void TriggerStatusEffectVisuals()
    {
        foreach (StatusEffectVisual visual in statusEffectVisuals)
        {
            if (visual.Effect.Triggered)
            {
                visual.Triggered();
            }
        }
    }

    [Serializable]
    private class StatusEffectVisual
    {
        public StatusEffect Effect => effect;
        public ParticleSystem Particles => particles;

        [SerializeField] private StatusEffect effect;
        [SerializeField] private ParticleSystem particles;
        [SerializeField] private ParticleSystem burst;

        public void Toggle(bool enabled)
        {
            if (!particles.isPlaying && enabled)
            {
                particles.Play();
            }
            else if (particles.isPlaying && !enabled)
            {
                particles.Stop();
            }
        }

        public void Triggered()
        {
            burst.Play();
        }
    }
}
