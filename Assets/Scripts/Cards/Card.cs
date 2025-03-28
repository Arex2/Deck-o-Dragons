using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The main card class that every single card uses. <para/>
/// Does nothing on it's own and needs a <see cref="CardComponent"/> (or multiple) to work.
/// </summary>
// Script by Ruben
[CreateAssetMenu(menuName = "Cards/Create New Card")]
public class Card : ScriptableObject
{
    [SerializeField] private int cost;
    [SerializeField] private Element element;
    [SerializeField] private CardCategory category;
    [SerializeField] private CardDiscardMethod immuneToDiscard;

    [Space]
    [SerializeField] private CardTag[] tags;

    [Space]
    [SerializeField] private string displayName;
    [SerializeField] private string description;

    [HideInInspector]
    [SerializeField] private CardComponent[] cardComponents;
    private Dictionary<Type, CardComponent[]> _cardComponentDictionary = new();

    public void OnLoad()
    {
        Dictionary<Type, List<CardComponent>> temp = new();

        foreach (CardComponent cardComponent in cardComponents)
        {
            Type type = cardComponent.GetType();

            if (!HasCardComponent(type))
            {
                temp.Add(type, new());
            }

            temp[type].Add(cardComponent);

            cardComponent.Initialize();
        }

        _cardComponentDictionary = new();

        foreach (var pair in temp)
        {
            _cardComponentDictionary.Add(pair.Key, pair.Value.ToArray());
        }
    }

    #region GetCardComponent Methods
    public T GetCardComponent<T>() where T : CardComponent
    {
        return GetCardComponents<T>()[0];
    }

    public CardComponent GetCardComponent(Type type)
    {
        return GetCardComponents(type)[0];
    }

    public T[] GetCardComponents<T>() where T : CardComponent
    {
        return GetCardComponents(typeof(T)) as T[];
    }

    public CardComponent[] GetCardComponents(Type type)
    {
        return _cardComponentDictionary[type];
    }

    public bool TryGetCardComponent<T>(out T cardComponent) where T : CardComponent
    {
        return TryGetCardComponent(out cardComponent);
    }

    public bool TryGetCardComponent(Type type, out CardComponent cardComponent)
    {
        bool success = TryGetCardComponents(type, out CardComponent[] result);

        if (success)
        {
            cardComponent = result[0];
        }
        else
        {
            cardComponent = null;
        }

        return success;
    }

    public bool TryGetCardComponents<T>(out T[] cardComponents) where T : CardComponent
    {
        return TryGetCardComponents(out cardComponents);
    }

    public bool TryGetCardComponents(Type type, out CardComponent[] cardComponents)
    {
        return _cardComponentDictionary.TryGetValue(type, out cardComponents);
    }

    public bool HasCardComponent<T>()
    {
        return HasCardComponent(typeof(T));
    }

    public bool HasCardComponent(Type type)
    {
        return _cardComponentDictionary.ContainsKey(type);
    }
    #endregion
}