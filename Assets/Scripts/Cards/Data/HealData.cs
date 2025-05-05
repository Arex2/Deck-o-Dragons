/// <summary>
/// Data for a heal.
/// </summary>
public class HealData
{
    public float Healing { get; set; }

    public void Negate()
    {
        Healing = 0;
    }

    public HealData(float healing)
    {
        Healing = healing;
    }

    public static implicit operator float(HealData data) => data.Healing;

    public static HealData operator +(HealData data, float value)
    {
        data.Healing += value;
        return data;
    }

    public static HealData operator -(HealData data, float value)
    {
        data.Healing -= value;
        return data;
    }

    public static HealData operator *(HealData data, float value)
    {
        data.Healing *= value;
        return data;
    }

    public static HealData operator /(HealData data, float value)
    {
        data.Healing /= value;
        return data;
    }
}
