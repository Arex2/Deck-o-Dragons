using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Data used by a <see cref="Card"/> when it's played.
/// </summary>
public struct CardData : IEnumerable<KeyValuePair<Type, Dictionary<string, object>>>
{
    private Dictionary<Type, Dictionary<string, object>> _data;

    public void SetCardData<T>(string key, T value)
    {
        if (_data == null)
        {
            _data = new();
        }

        Type type = typeof(T);

        if (!_data.ContainsKey(type))
        {
            _data[type] = new();
        }

        _data[type][key] = value;
    }

    public bool HasCardData<T>(string key)
    {
        if (_data == null)
        {
            return false;
        }

        Type type = typeof(T);

        if (!_data.ContainsKey(type))
        {
            return false;
        }

        return _data[type].ContainsKey(key);
    }

    public T GetCardData<T>(string key) => GetCardData<T>(key, default);

    public T GetCardData<T>(string key, T defaultValue)
    {
        TryGetCardData(key, out T result, defaultValue);

        return result;
    }

    public bool TryGetCardData<T>(string key, out T value) => TryGetCardData(key, out value, default);

    public bool TryGetCardData<T>(string key, out T value, T defaultValue)
    {
        if (_data == null)
        {
            value = defaultValue;
            return false;
        }

        Type type = typeof(T);

        if (!_data.TryGetValue(type, out var dictionary))
        {
            value = defaultValue;
            return false;
        }

        if (!dictionary.TryGetValue(key, out object result))
        {
            value = defaultValue;
            return false;
        }

        value = (T)result;
        return true;
    }

    public IEnumerator<KeyValuePair<Type, Dictionary<string, object>>> GetEnumerator()
    {
        if (_data == null)
        {
            _data = new();
        }

        return _data.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
