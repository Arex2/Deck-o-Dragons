using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object that can be targetted by a <see cref="Card"/> or enemy.
/// </summary>
// Script by Ruben
public abstract class Target : MonoBehaviour
{
    public abstract Team Team { get; }
    public float MaxHP => maxHp;
    public float HP => hp;

    public bool IsLeader => isLeader;

    [SerializeField] private float maxHp;
    protected float hp;

    [SerializeField] private bool isLeader;

    [SerializeField] private List<StatusEffect> immuneToStatusEffects;

    public Dictionary<StatusEffect, StatusEffectData> StatusEffectsData => _statusEffectsData;

    private Dictionary<StatusEffect, StatusEffectData> _statusEffectsData = new();

    protected virtual void OnEnable()
    {
        TargetManager.AddTarget(this);
    }

    protected virtual void OnDisable()
    {
        TargetManager.RemoveTarget(this);
    }

    protected virtual void Awake()
    {
        hp = maxHp;
    }

    protected virtual void Start()
    {

    }

    public virtual void Hurt(Target attacker, float amount)
    {
        foreach (var pair in _statusEffectsData)
        {
            pair.Key.Setup(this, pair.Value);

            pair.Key.OnHurt(attacker, ref amount);
        }

        RemoveFinishedStatusEffects();

        Hurt(amount);
    }

    public virtual void Hurt(float amount)
    {
        if (amount < 0)
        {
            amount = 0;
        }

        hp -= amount;

        if (hp < 0)
        {
            hp = 0;
        }

        Debug.Log(name + " has taken " + amount + " damage");
        UpdateHP();
    }

    public virtual void Heal(float amount)
    {
        foreach (var pair in _statusEffectsData)
        {
            pair.Key.Setup(this, pair.Value);

            pair.Key.OnHeal(ref amount);
        }

        RemoveFinishedStatusEffects();

        if (amount < 0)
        {
            amount = 0;
        }

        hp += amount;

        if (hp > maxHp)
        {
            hp = maxHp;
        }

        Debug.Log(name + " has healed " + amount + " HP");
        UpdateHP();
    }

    protected virtual void UpdateHP()
    {

    }

    public abstract Bounds GetWorldBounds();

    protected virtual void UpdateStatusEffects()
    {

    }

    public virtual void OnTurnStart()
    {
        foreach (var pair in _statusEffectsData)
        {
            pair.Key.Setup(this, pair.Value);

            pair.Key.OnTurnStart();
        }

        RemoveFinishedStatusEffects();
    }

    public virtual void OnTurnEnd()
    {
        foreach (var pair in _statusEffectsData)
        {
            pair.Key.Setup(this, pair.Value);

            pair.Key.OnTurnEnd();
        }

        RemoveFinishedStatusEffects();
    }

    public virtual void DoAttack(Target target, ref float damage)
    {
        foreach (var pair in _statusEffectsData)
        {
            pair.Key.Setup(this, pair.Value);

            pair.Key.OnAttack(target, ref damage);
        }

        RemoveFinishedStatusEffects();
    }

    private void RemoveFinishedStatusEffects()
    {
        ClearStatusEffectsWithPredicate((pair) => pair.Value.Duration <= 0);
    }

    public void ApplyStatusEffect(StatusEffect statusEffect, int potency, int duration, bool invokeOnOtherStatusEffectApplied = true) => ApplyStatusEffect(statusEffect, new(potency, duration), invokeOnOtherStatusEffectApplied);

    public void ApplyStatusEffect(StatusEffect statusEffect, StatusEffectData data, bool invokeOnOtherStatusEffectApplied = true)
    {
        if (immuneToStatusEffects.Contains(statusEffect))
        {
            return;
        }

        if (invokeOnOtherStatusEffectApplied)
        {
            foreach (var pair in _statusEffectsData)
            {
                pair.Key.Setup(this, pair.Value);

                pair.Key.OnOtherStatusEffectApplied(statusEffect, data);
            }
        }

        _statusEffectsData[statusEffect] = data;

        statusEffect.Setup(this, data);
        statusEffect.OnApplied();

        //data.OnChanged += UpdateStatusEffects;

        RemoveFinishedStatusEffects();
    }

    public StatusEffectData GetStatusEffectData(StatusEffect statusEffect)
    {
        if (!TryGetStatusEffectData(statusEffect, out StatusEffectData data))
        {
            return null;
        }

        return data;
    }

    public bool TryGetStatusEffectData(StatusEffect statusEffect, out StatusEffectData data) => _statusEffectsData.TryGetValue(statusEffect, out data);

    public bool HasStatusEffect(StatusEffect statusEffect) => _statusEffectsData.ContainsKey(statusEffect);

    public void RemoveStatusEffect(StatusEffect statusEffect)
    {
        if (!_statusEffectsData.TryGetValue(statusEffect, out StatusEffectData data))
        {
            return;
        }

        statusEffect.Setup(this, data);
        statusEffect.OnRemoved();

        //data.OnChanged -= UpdateStatusEffects;

        _statusEffectsData.Remove(statusEffect);

        UpdateStatusEffects();
    }

    public void ClearAllDebuffs()
    {
        ClearStatusEffectsWithPredicate((pair) => pair.Key.IsDebuff);
    }

    public void ClearAllNonDebuffs()
    {
        ClearStatusEffectsWithPredicate((pair) => !pair.Key.IsDebuff);
    }

    private void ClearStatusEffectsWithPredicate(Func<KeyValuePair<StatusEffect, StatusEffectData>, bool> predicate)
    {
        List<StatusEffect> statusEffectsToRemove = new();

        foreach (var pair in _statusEffectsData)
        {
            if (predicate.Invoke(pair))
            {
                pair.Key.Setup(this, pair.Value);

                pair.Key.OnRemoved();
                statusEffectsToRemove.Add(pair.Key);

                //pair.Value.OnChanged -= UpdateStatusEffects;
            }
        }

        foreach (var statusEffect in statusEffectsToRemove)
        {
            _statusEffectsData.Remove(statusEffect);
        }

        UpdateStatusEffects();
    }

    public void ClearAllStatusEffects()
    {
        foreach (var pair in _statusEffectsData)
        {
            pair.Key.Setup(this, pair.Value);

            pair.Key.OnRemoved();

            //pair.Value.OnChanged -= UpdateStatusEffects;
        }

        _statusEffectsData.Clear();

        UpdateStatusEffects();
    }
}
