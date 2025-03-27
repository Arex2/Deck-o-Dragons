/// <summary>
/// Generic class that displays a variable as a toggleable field
/// </summary>
[System.Serializable]
public class Optional<T>
{
    public bool Enabled;
    public T Value;

    public void Set(T value) => Value = value;

    public static implicit operator bool(Optional<T> obj) => obj.Enabled;
    public static implicit operator T(Optional<T> obj) => obj.Value;
    public static implicit operator Optional<T>(T obj) => new Optional<T>(obj);

    public Optional()
    {

    }

    public Optional(T value) : this()
    {
        Set(value);
    }

    public Optional(bool enabled) : this()
    {
        Enabled = enabled;
    }

    public Optional(bool enabled, T value) : this(enabled)
    {
        Set(value);
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}