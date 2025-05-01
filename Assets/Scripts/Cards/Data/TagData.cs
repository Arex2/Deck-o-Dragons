using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Struct for storing <see cref="CardTag"/>s and their potency.
/// </summary>
[Serializable]
public struct TagData : IEnumerable<CardTag>
{
    public CardTag this[int index] => Tags[index];

    public float this[CardTag tag] => GetPotency(tag);

    [SerializeField] private TagDataEntry[] data;

    public List<CardTag> Tags
    {
        get
        {
            TryCache();
            return _tags;
        }
    }
    [NonSerialized]
    private List<CardTag> _tags;

    public int Count
    {
        get
        {
            TryCache();
            return _count;
        }
    }
    [NonSerialized]
    private int _count;

    [NonSerialized]
    private Dictionary<CardTag, float> _dictionary;

    [NonSerialized]
    private bool _cached;

    private void TryCache()
    {
        if (_cached)
        {
            return;
        }

        _cached = true;

        _dictionary = new();
        _tags = new();
        _count = 0;

        if (data == null)
        {
            return;
        }

        foreach (TagDataEntry entry in data)
        {
            CardTag tag = entry.Tag;

            SetInternal(tag, tag.HasPotency ? entry.Potency : 0, false);
        }
    }

    public void SetTag(CardTag tag) => SetPotency(tag, 0);

    public void SetPotency(CardTag tag, float potency)
    {
        SetInternal(tag, potency, true);
    }

    private void SetInternal(CardTag tag, float potency, bool tryCache)
    {
        if (tag == null)
        {
            return;
        }

        if (tryCache)
        {
            TryCache();
        }

        if (_dictionary.ContainsKey(tag))
        {
            _dictionary[tag] = potency;
            return;
        }

        _tags.Add(tag);
        _count++;

        _dictionary.Add(tag, potency);
    }

    public void RemoveTag(CardTag tag)
    {
        if (!_dictionary.Remove(tag))
        {
            return;
        }

        _tags.Remove(tag);
        _count--;
    }

    public bool HasTag(CardTag tag)
    {
        TryCache();

        if (tag == null)
        {
            return false;
        }

        return _dictionary.ContainsKey(tag);
    }

    public float GetPotency(CardTag tag)
    {
        if (TryGet(tag, out float result))
        {
            return result;
        }

        return 0;
    }

    public bool TryGet(CardTag tag, out float potency)
    {
        TryCache();

        return _dictionary.TryGetValue(tag, out potency);
    }

    public void Merge(TagData otherData)
    {
        TryCache();
        otherData.TryCache();

        foreach (var pair in otherData._dictionary)
        {
            SetInternal(pair.Key, pair.Value, false);
        }
    }

    public IEnumerator<CardTag> GetEnumerator()
    {
        return ((IEnumerable<CardTag>)Tags).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Tags.GetEnumerator();
    }

    public static implicit operator List<CardTag>(TagData data) => data.Tags;

    [Serializable]
    private class TagDataEntry
    {
        public CardTag Tag => tag;
        public float Potency => potency;

        [SerializeField] private CardTag tag;
        [SerializeField] private float potency;
    }
}
