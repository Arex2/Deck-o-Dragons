using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Is attached to a <see cref="Card"/> and responsible for giving functionality to a card.
/// </summary>
// Script by Ruben
public abstract class CardComponent : ScriptableObject
{
#if UNITY_EDITOR
#pragma warning disable 414
    [HideInInspector]
    [SerializeField] private bool expandedInEditor = true;
#pragma warning restore 414

    [HideInInspector]
    [SerializeField] private int orderInEditor;
#endif

    private delegate string ReplaceDescriptionKeywordDelegate();
    private Dictionary<string, ReplaceDescriptionKeywordDelegate> _keywordReplacementDelegates = null;

    public virtual TargetFilter TargetFilter => null;

    public int Tier => card.Tier;

    /// <summary>
    /// The current <see cref="Target"/> that's using this <see cref="Card"/>.
    /// </summary>
    public Target User => card.User;

    /// <summary>
    /// Wether or not this <see cref="CardComponent"/> is enabled and therefore does something.
    /// </summary>
    public bool Enabled
    {
        get => _enabled;
        set => _enabled = value;
    }

    [HideInInspector]
    [SerializeField] protected Card card;
    [HideInInspector]
    [SerializeField] private bool disableOnStart;
    private bool _enabled;

    public void InternalInitialize()
    {
        _enabled = !disableOnStart;

        foreach (MethodInfo methodInfo in GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            ReplaceDescriptionKeywordDelegate @delegate = null;

            // Loop through all modify card description attributes the method has
            foreach (ReplaceDescriptionKeywordAttribute attribute in methodInfo.GetCustomAttributes<ReplaceDescriptionKeywordAttribute>())
            {
                if (attribute == null)
                {
                    continue;
                }

                // Should return float
                if (methodInfo.ReturnType != typeof(string))
                {
                    continue;
                }

                // Should have no parameters
                if (methodInfo.GetParameters().Length != 0)
                {
                    continue;
                }

                // Add to collection
                if (@delegate == null)
                {
                    @delegate = (ReplaceDescriptionKeywordDelegate)methodInfo.CreateDelegate(typeof(ReplaceDescriptionKeywordDelegate), this);
                }

                if (_keywordReplacementDelegates == null)
                {
                    _keywordReplacementDelegates = new();
                }

                _keywordReplacementDelegates.Add((string.IsNullOrEmpty(attribute.Keyword) ? name : attribute.Keyword).ToLower().Trim(), @delegate);
            }
        }

        Initialize();
    }

    public virtual void Initialize()
    {

    }

    public bool ShouldReplaceDescriptionKeywords() => _keywordReplacementDelegates != null;

    public string ReplaceDescriptionKeyword(string keyword)
    {
        if (!_keywordReplacementDelegates.TryGetValue(keyword, out ReplaceDescriptionKeywordDelegate @delegate))
        {
            return null;
        }

        return @delegate.Invoke();
    }

    #region GetCardComponent Methods
    public T GetCardComponent<T>() where T : CardComponent => card.GetCardComponent<T>();

    public CardComponent GetCardComponent(Type type) => card.GetCardComponent(type);

    public T[] GetCardComponents<T>() where T : CardComponent => card.GetCardComponents<T>();

    public CardComponent[] GetCardComponents(Type type) => card.GetCardComponents(type);

    public bool TryGetCardComponent<T>(out T cardComponent) where T : CardComponent => card.TryGetCardComponent(out cardComponent);

    public bool TryGetCardComponent(Type type, out CardComponent cardComponent) => card.TryGetCardComponent(type, out cardComponent);

    public bool TryGetCardComponents<T>(out T[] cardComponents) where T : CardComponent => card.TryGetCardComponents(out cardComponents);

    public bool TryGetCardComponents(Type type, out CardComponent[] cardComponents) => card.TryGetCardComponents(type, out cardComponents);

    public bool HasCardComponent<T>() => card.HasCardComponent(typeof(T));

    public bool HasCardComponent(Type type) => card.HasCardComponent(type);
    #endregion
}