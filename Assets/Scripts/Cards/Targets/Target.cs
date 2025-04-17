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

    public IEnumerable<StatusEffect> StatusEffects => _statusEffectsData.Keys;

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
            pair.Key.OnHurt(this, pair.Value, attacker, ref amount);
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

        //Debug.Log(name + " has taken " + amount + " damage");
        UpdateHP();
    }

    public virtual void Heal(float amount)
    {
        foreach (var pair in _statusEffectsData)
        {
            pair.Key.OnHeal(this, pair.Value, ref amount);
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

        //Debug.Log(name + " has healed " + amount + " HP");
        UpdateHP();
    }

    protected virtual void UpdateHP()
    {

    }

    public abstract Bounds GetWorldBounds();

    public virtual void DoAttack(Target target, ref float damage)
    {
        foreach (var pair in _statusEffectsData)
        {
            pair.Key.OnAttack(this, pair.Value, target, ref damage);
        }

        RemoveFinishedStatusEffects();
    }

    public void RemoveFinishedStatusEffects()
    {
        ClearStatusEffectWithPredicate((pair) => pair.Value.Duration <= 0);
    }

    public void ApplyStatusEffect(StatusEffect statusEffect, int potency, int duration) => ApplyStatusEffect(statusEffect, new(potency, duration));

    public void ApplyStatusEffect(StatusEffect statusEffect, StatusEffectData data)
    {
        _statusEffectsData[statusEffect] = data;

        statusEffect.OnApplied(this, data);
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

        statusEffect.OnRemoved(this, data);

        _statusEffectsData.Remove(statusEffect);
    }

    public void ClearAllDebuffs()
    {
        ClearStatusEffectWithPredicate((pair) => pair.Key.IsDebuff);
    }

    public void ClearAllNonDebuffs()
    {
        ClearStatusEffectWithPredicate((pair) => !pair.Key.IsDebuff);
    }

    private void ClearStatusEffectWithPredicate(Func<KeyValuePair<StatusEffect, StatusEffectData>, bool> predicate)
    {
        List<StatusEffect> statusEffectsToRemove = new();

        foreach (var pair in _statusEffectsData)
        {
            if (predicate.Invoke(pair))
            {
                pair.Key.OnRemoved(this, pair.Value);
                statusEffectsToRemove.Add(pair.Key);
            }
        }

        foreach (var statusEffect in statusEffectsToRemove)
        {
            _statusEffectsData.Remove(statusEffect);
        }
    }

    public void ClearAllStatusEffects()
    {
        foreach (var pair in _statusEffectsData)
        {
            pair.Key.OnRemoved(this, pair.Value);
        }

        _statusEffectsData.Clear();
    }
}
