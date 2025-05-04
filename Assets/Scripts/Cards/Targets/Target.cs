using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object that can be targetted by a <see cref="Card"/>.
/// </summary>
// Script by Ruben
public abstract class Target : MonoBehaviour
{
    public static Action<Team> TurnStart { get; set; }
    public static Action<Team> TurnEnd { get; set; }

    public abstract Team Team { get; }
    public float MaxHP
    {
        get => maxHp;
        set => maxHp = value;
    }
    public float HP {
        get => hp;
        set
        {
            if (hp == value)
            {
                return;
            }

            hp = value;

            if (hp < 0)
            {
                hp = 0;
            }
            else if (hp > maxHp)
            {
                hp = maxHp;
            }

            UpdateHP();
        }
    }

    public bool Dead => _dead;
    private bool _dead;

    public bool IsLeader => isLeader;

    [SerializeField] private float maxHp;
    private float hp;

    [SerializeField] private bool isLeader;

    [SerializeField] private List<StatusEffect> immuneToStatusEffects;

    public bool NotifyStatusEffects
    {
        get => _notifyStatusEffects;
        set => _notifyStatusEffects = value;
    }

    private bool _notifyStatusEffects = true;

    public Dictionary<StatusEffect, StatusEffectData> StatusEffectsData => _statusEffectsData;

    private Dictionary<StatusEffect, StatusEffectData> _statusEffectsData = new();

    private ITargetCallbacks[] _targetCallbacks;

    protected virtual void OnEnable()
    {
        TargetManager.AddTarget(this);

        TurnStart += EvaluateTurnStart;
        TurnEnd += EvaluateTurnEnd;
    }

    protected virtual void OnDisable()
    {
        TargetManager.RemoveTarget(this);

        TurnStart -= EvaluateTurnStart;
        TurnEnd -= EvaluateTurnEnd;
    }

    private void EvaluateTurnStart(Team team)
    {
        if (team != Team)
        {
            return;
        }

        OnTurnStart();
    }

    private void EvaluateTurnEnd(Team team)
    {
        if (team != Team)
        {
            return;
        }

        OnTurnEnd();
    }

    protected virtual void Awake()
    {
        hp = maxHp;

        _targetCallbacks = GetComponentsInChildren<ITargetCallbacks>(true);
    }

    protected virtual void Start()
    {

    }

    public virtual void Hurt(Target attacker, AttackData attackData)
    {
        if (!Dead && _notifyStatusEffects)
        {
            foreach (var pair in _statusEffectsData)
            {
                pair.Key.Setup(this, pair.Value);

                pair.Key.OnHurt(attacker, attackData);
            }

            RemoveFinishedStatusEffects();
        }

        Hurt(attackData);
    }

    public virtual void Hurt(AttackData attackData)
    {
        if (Dead)
        {
            return;
        }

        if (attackData > 0)
        {
            HP -= attackData;
        }
        else
        {
            // Miss!
        }

        if (hp <= 0)
        {
            OnDeath();
        }
        else
        {
            foreach (ITargetCallbacks callbacks in _targetCallbacks)
            {
                callbacks.OnHurt();
            }
        }

        //Debug.Log(name + " has taken " + amount + " damage");
    }

    public virtual void Heal(HealData healData)
    {
        if (Dead)
        {
            return;
        }

        if (_notifyStatusEffects)
        {
            foreach (var pair in _statusEffectsData)
            {
                pair.Key.Setup(this, pair.Value);

                pair.Key.OnHeal(healData);
            }

            RemoveFinishedStatusEffects();
        }

        if (healData > 0)
        {
            HP += healData;
        }

        foreach (ITargetCallbacks callbacks in _targetCallbacks)
        {
            callbacks.OnHeal();
        }

        //Debug.Log(name + " has healed " + amount + " HP");
    }

    public virtual void OnDeath()
    {
        _dead = true;

        foreach (ITargetCallbacks callbacks in _targetCallbacks)
        {
            callbacks.OnDeath();
        }
    }


    protected virtual void UpdateHP()
    {
        foreach (ITargetCallbacks callbacks in _targetCallbacks)
        {
            callbacks.OnUpdateHP();
        }
    }

    protected virtual void UpdateStatusEffects()
    {
        foreach (ITargetCallbacks callbacks in _targetCallbacks)
        {
            callbacks.OnUpdateStatusEffects();
        }
    }

    public abstract Bounds GetWorldBounds();
    public abstract Vector2 GetHitPosition();

    public virtual void OnTurnStart()
    {
        if (Dead)
        {
            return;
        }

        foreach (var pair in _statusEffectsData)
        {
            pair.Key.Setup(this, pair.Value);

            pair.Key.OnTurnStart();
        }

        foreach (ITargetCallbacks callbacks in _targetCallbacks)
        {
            callbacks.OnTurnStart();
        }

        RemoveFinishedStatusEffects();
    }

    public virtual void OnTurnEnd()
    {
        if (Dead)
        {
            return;
        }

        foreach (var pair in _statusEffectsData)
        {
            pair.Key.Setup(this, pair.Value);

            pair.Key.OnTurnEnd();
        }

        foreach (ITargetCallbacks callbacks in _targetCallbacks)
        {
            callbacks.OnTurnEnd();
        }

        RemoveFinishedStatusEffects();
    }

    public virtual void DoAttack(Target target, AttackData attackData)
    {
        foreach (var pair in _statusEffectsData)
        {
            pair.Key.Setup(this, pair.Value);

            pair.Key.OnAttack(target, attackData);
        }

        RemoveFinishedStatusEffects();
    }

    private void RemoveFinishedStatusEffects()
    {
        ClearStatusEffectsWithPredicate((pair) => pair.Value.Duration <= 0);
    }

    public void ApplyStatusEffect(StatusEffect statusEffect, int potency, int duration) => ApplyStatusEffect(statusEffect, new(potency, duration));

    public void ApplyStatusEffect(StatusEffect statusEffect, StatusEffectData data)
    {
        if (immuneToStatusEffects.Contains(statusEffect))
        {
            return;
        }

        if (_notifyStatusEffects)
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
