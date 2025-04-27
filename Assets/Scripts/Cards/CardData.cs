using System;
using System.Reflection;
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
        SetCardData(key, typeof(T), value);
    }

    public void SetCardData(string key, Type type, object value)
    {
        if (_data == null)
        {
            _data = new();
        }

        if (!_data.ContainsKey(type))
        {
            _data[type] = new();
        }

        _data[type][key] = value;
    }

    public bool HasCardData<T>(string key)
    {
        return HasCardData(key, typeof(T));
    }

    public bool HasCardData(string key, Type type)
    {
        if (_data == null)
        {
            return false;
        }

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

    public bool TryGetCardData<T>(string key, out T value) => TryGetCardData(key, out value, default(T));

    public bool TryGetCardData(string key, Type type, out object value) => TryGetCardData(key, type, out value, GetDefault(type));

    private static object GetDefault(Type t)
    {
        if (t == typeof(float)) return default(float);
        if (t == typeof(int)) return default(int);
        if (t == typeof(bool)) return default(bool);
        if (t == typeof(string)) return default(string);

        return _getDefaultGenericMethodInfo.MakeGenericMethod(t).Invoke(null, null);
    }

    private static readonly MethodInfo _getDefaultGenericMethodInfo = typeof(CardData).GetType().GetMethod("GetDefaultGeneric", BindingFlags.Static | BindingFlags.NonPublic);

    private static T GetDefaultGeneric<T>() =>  default(T);

    public bool TryGetCardData<T>(string key, out T value, T defaultValue)
    {
        bool success = TryGetCardData(key, typeof(T), out object result, defaultValue);
        value = (T)result;

        return success;
    }

    public bool TryGetCardData(string key, Type type, out object value, object defaultValue)
    {
        if (_data == null)
        {
            value = defaultValue;
            return false;
        }

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

        value = result;
        return true;
    }

    /// <summary>
    /// Merges this <see cref="CardData"/> with some <paramref name="other"/> <see cref="CardData"/>.
    /// </summary>
    public void Merge(CardData other)
    {
        // Good naming schemes have gone out of the window...
        foreach (var pair1 in other)
        {
            foreach (var pair2 in pair1.Value)
            {
                SetCardData(pair2.Key, pair1.Key, pair2.Value);
            }
        }
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
