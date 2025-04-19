using UnityEngine;

/// <summary>
/// The base class for all status effects in the game.
/// </summary>
public abstract class StatusEffect : GUIDScriptableObject
{
    public const string ASSET_MENU_PATH = "Cards/Status Effects/";

    public abstract bool HasPotency { get; }

    public virtual string PotencyName => null;

    public virtual string DurationName => null;

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

    public Target User { get; set; }
    public StatusEffectData Data { get; set; }

    /// <summary>
    /// How strong the <see cref="StatusEffect"/> is.
    /// </summary>
    public float Potency
    {
        get => Data == null ? 0 : Data.Potency;
        set
        {
            if (Data == null)
            {
                return;
            }

            Data.Potency = value;
        }
    }

    /// <summary>
    /// How long the <see cref="StatusEffect"/> lasts.
    /// </summary>
    public int Duration
    {
        get => Data == null ? 0 : Data.Duration;
        set
        {
            if (Data == null)
            {
                return;
            }

            Data.Duration = value;
        }
    }

    public void Setup(Target target, StatusEffectData data)
    {
        User = target;
        Data = data;
    }

    public virtual void OnApplied()
    {
        
    }

    public virtual void OnRemoved()
    {

    }

    public virtual void OnTurnStart()
    {

    }

    public virtual void OnTurnEnd()
    {

    }

    public virtual void OnAttack(Target target, ref float amount)
    {

    }

    public virtual void OnHurt(Target attacker, ref float amount)
    {

    }

    public virtual void OnHeal(ref float healing)
    {

    }

    public virtual void OnOtherStatusEffectApplied(StatusEffect statusEffect, StatusEffectData otherData)
    {

    }
}
