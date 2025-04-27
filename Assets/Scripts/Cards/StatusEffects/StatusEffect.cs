using System;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// The base class for every status effect in the game.
/// </summary>
public abstract class StatusEffect : GUIDScriptableObject
{
    public const string ASSET_MENU_PATH = "Cards/Status Effects/";

    private static readonly Regex _potencyRegex = new Regex(@"\{potency\}", RegexOptions.IgnoreCase);
    private static readonly Regex _durationRegex = new Regex(@"\{duration\}", RegexOptions.IgnoreCase);
    private static readonly Regex _userDataRegex = new Regex(@"\{(user|extra|other)data\}", RegexOptions.IgnoreCase);

    public abstract bool HasPotency { get; }

    public virtual bool? ForcedPotencyIsPercent => null;

    public virtual string PotencyName => null;

    public virtual string DurationName => null;

    public Sprite Icon => icon;
    public string DisplayName
    {
        get
        {
            if (string.IsNullOrEmpty(_cachedDisplayName))
            {
                _cachedDisplayName = string.IsNullOrEmpty(displayName) ? name : displayName;
            }

            return _cachedDisplayName;
        }
    }
    [NonSerialized]
    private string _cachedDisplayName;
    public bool IsDebuff => isDebuff;
    public bool PotencyIsPercent => ForcedPotencyIsPercent.HasValue ? ForcedPotencyIsPercent.Value : potencyIsPercent;

    [NonSerialized]
    private string _descriptionFormat = null;

    [SerializeField] private Sprite icon;
    [SerializeField] private string displayName;
    [TextArea(1, 5)]
    [SerializeField] private string description;
    [SerializeField] private bool hidden;
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

    /// <summary>
    /// Custom generic data for a <see cref="StatusEffect"/>.
    /// </summary>
    public object UserData
    {
        get => Data == null ? null : Data.UserData;
        set
        {
            if (Data == null)
            {
                return;
            }

            Data.UserData = value;
        }
    }

    /// <summary>
    /// Wether or not <see cref="UserData"/> has been setup properly.
    /// </summary>
    public bool SetupUserData => Data == null ? false : Data.SetupUserData;

    public string GetDescription(StatusEffectData data)
    {
        return GetDescription(data.Potency, data.Duration, data.UserData);
    }

    public string GetDescription(float potency, int duration, object userData)
    {
        if (string.IsNullOrEmpty(_descriptionFormat))
        {
            _descriptionFormat = _userDataRegex.Replace(_durationRegex.Replace(_potencyRegex.Replace(description, "{0}"), "{1}"), "{2}");
        }

        string potencyString = GetPotencyString(potency, PotencyIsPercent);

        return string.Format(_descriptionFormat, potencyString, duration, GetUserDataString(userData));
    }

    public virtual string GetUserDataString(object userData)
    {
        if (userData == null)
        {
            return DefaultUserDataString();
        }

        return userData.ToString();
    }

    public virtual string DefaultUserDataString() => "null";

    public static string GetPotencyString(float potency, bool potencyIsPercent)
    {
        if (potencyIsPercent)
        {
            int count = BitConverter.GetBytes(decimal.GetBits((decimal)potency)[3])[2];
            string format = "P";

            if (count > 2)
            {
                format += count - 2;
            }
            else
            {
                format += "0";
            }

            return potency.ToString(format, CultureInfo.InvariantCulture);
        }
        else
        {
            return potency.ToString();
        }
    }

    protected void Remove()
    {
        Duration = 0;
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
