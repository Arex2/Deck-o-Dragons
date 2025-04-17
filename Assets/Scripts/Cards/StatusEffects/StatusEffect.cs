using UnityEngine;

public abstract class StatusEffect : GUIDScriptableObject
{
    public const string ASSET_MENU_PATH = "Cards/Status Effects/";

    public abstract bool HasPotency { get; }

    public abstract string PotencyName { get; }

    public abstract string DurationName { get; }

    public Sprite Icon => icon;
    public string DisplayName => displayName;
    public string Description => description;
    public bool IsDebuff => isDebuff;
    public bool PotencyIsPercent => potencyIsPercent;

    [SerializeField] private Sprite icon;
    [SerializeField] private string displayName;
    [TextArea(1, 5)]
    [SerializeField] private string description;
    [SerializeField] private bool isDebuff;
    [SerializeField] private bool potencyIsPercent;

    public virtual void OnApplied(Target user, StatusEffectData data)
    {
        
    }

    public virtual void OnRemoved(Target user, StatusEffectData data)
    {

    }

    public virtual void OnTurnStart(Target user, StatusEffectData data)
    {

    }

    public virtual void OnTurnEnd(Target user, StatusEffectData data)
    {

    }

    public virtual void OnAttack(Target user, StatusEffectData data, Target target, ref float amount)
    {

    }

    public virtual void OnHurt(Target user, StatusEffectData data, Target attacker, ref float amount)
    {

    }

    public virtual void OnHeal(Target user, StatusEffectData data, ref float healing)
    {

    }

    public virtual void OnDoApplyStatusEffect(Target user, StatusEffectData data, StatusEffect statusEffect, StatusEffectData otherData)
    {

    }
}
