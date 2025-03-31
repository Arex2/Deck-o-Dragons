using System;
using System.Collections;
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

    [HideInInspector]
    [SerializeField] protected Card card;

    public virtual void Initialize()
    {

    }

    public abstract IEnumerator Play();

    public string ModifyCardDescription(string description)
    {
        return description;
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