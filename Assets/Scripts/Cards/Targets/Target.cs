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

    /*
    public CardData CardData => _statusEffectCardData;
    private CardData _statusEffectCardData;
    */

    public bool NotifyStatusEffects
    {
        get => _notifyStatusEffects;
        set => _notifyStatusEffects = value;
    }

    private bool _notifyStatusEffects = true;

    public List<StatusEffectData> StatusEffects => _statusEffects;

    private List<StatusEffectData> _statusEffects = new();
    private Dictionary<StatusEffect, List<StatusEffectData>> _statusEffectDictionary = new();

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
            foreach (StatusEffectData data in _statusEffects)
            {
                data.Setup(this);

                data.StatusEffect.OnHurt(attacker, attackData);
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
            foreach (StatusEffectData data in _statusEffects)
            {
                data.Setup(this);

                data.StatusEffect.OnHeal(healData);
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

        foreach (StatusEffectData data in _statusEffects)
        {
            data.Setup(this);

            data.StatusEffect.OnTurnStart();
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

        foreach (StatusEffectData data in _statusEffects)
        {
            data.Setup(this);

            data.StatusEffect.OnTurnEnd();

            data.Duration--;
        }

        foreach (ITargetCallbacks callbacks in _targetCallbacks)
        {
            callbacks.OnTurnEnd();
        }

        RemoveFinishedStatusEffects();
    }

    public virtual void DoAttack(Target target, AttackData attackData)
    {
        foreach (StatusEffectData data in _statusEffects)
        {
            data.Setup(this);

            data.StatusEffect.OnAttack(target, attackData);
        }

        RemoveFinishedStatusEffects();
    }

    private void RemoveFinishedStatusEffects()
    {
        ClearStatusEffectsWithPredicate((data) => data.Duration <= 0);
    }

    public void AddStatusEffect(StatusEffect statusEffect, int potency, int duration) => AddStatusEffect(new(statusEffect, potency, duration));

    public void AddStatusEffect(StatusEffectData newData)
    {
        if (immuneToStatusEffects.Contains(newData.StatusEffect))
        {
            return;
        }

        if (_notifyStatusEffects)
        {
            foreach (StatusEffectData data in _statusEffects)
            {
                data.Setup(this);

                data.StatusEffect.OnAddOtherStatusEffect(data);
            }
        }

        _statusEffects.Add(newData);

        StatusEffect statusEffect = newData.StatusEffect;

        if (!_statusEffectDictionary.ContainsKey(statusEffect))
        {
            _statusEffectDictionary.Add(statusEffect, new());
        }
        _statusEffectDictionary[statusEffect].Add(newData);

        if (_notifyStatusEffects)
        {
            newData.Setup(this);

            statusEffect.OnAdded();
        }

        //data.OnChanged += UpdateStatusEffects;

        RemoveFinishedStatusEffects();
    }

    public List<StatusEffectData> GetStatusEffectData(StatusEffect statusEffect)
    {
        if (!TryGetStatusEffectData(statusEffect, out List<StatusEffectData> list))
        {
            return null;
        }

        return list;
    }

    public bool TryGetStatusEffectData(StatusEffect statusEffect, out List<StatusEffectData> list) => _statusEffectDictionary.TryGetValue(statusEffect, out list);

    //public bool HasStatusEffect(StatusEffect statusEffect) => _statusEffectDictionary.ContainsKey(statusEffect) && _statusEffectDictionary[statusEffect].Count > 0;

    public void RemoveStatusEffect(StatusEffect statusEffect)
    {
        if (!TryGetStatusEffectData(statusEffect, out List<StatusEffectData> list))
        {
            return;
        }

        foreach (StatusEffectData data in list)
        {
            if (data != null)
            {
                continue;
            }

            data.Setup(this);

            statusEffect.OnRemoved();

            _statusEffects.Remove(data);
        }

        //data.OnChanged -= UpdateStatusEffects;

        _statusEffectDictionary[statusEffect].Clear();

        UpdateStatusEffects();
    }

    public void ClearAllDebuffs()
    {
        ClearStatusEffectsWithPredicate((data) => data.StatusEffect.IsDebuff);
    }

    public void ClearAllNonDebuffs()
    {
        ClearStatusEffectsWithPredicate((data) => !data.StatusEffect.IsDebuff);
    }

    private void ClearStatusEffectsWithPredicate(Func<StatusEffectData, bool> predicate)
    {
        List<StatusEffectData> dataToRemove = new();

        foreach (StatusEffectData data in _statusEffects)
        {
            if (predicate.Invoke(data))
            {
                data.Setup(this);

                data.StatusEffect.OnRemoved();

                dataToRemove.Add(data);

                //pair.Value.OnChanged -= UpdateStatusEffects;
            }
        }

        foreach (StatusEffectData data in dataToRemove)
        {
            _statusEffects.Remove(data);
            _statusEffectDictionary[data.StatusEffect].Remove(data);
        }

        UpdateStatusEffects();
    }

    public void ClearAllStatusEffects()
    {
        foreach (StatusEffectData data in _statusEffects)
        {
            data.Setup(this);

            data.StatusEffect.OnRemoved();

            //pair.Value.OnChanged -= UpdateStatusEffects;
        }

        _statusEffects.Clear();
        _statusEffectDictionary.Clear();

        UpdateStatusEffects();
    }
}
