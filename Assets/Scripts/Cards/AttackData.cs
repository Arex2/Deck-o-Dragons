public class AttackData
{
    public float Damage { get; set; }
    public bool Bullseye { get; set; }

    public string NegateMessage { get; set; } = null;

    public void Negate(string messsage = null)
    {
        Damage = 0;
        NegateMessage = messsage;
    }

    public AttackData(float damage) : this(damage, false)
    {

    }

    public AttackData(float damage, bool bullseye)
    {
        Damage = damage;
        Bullseye = bullseye;
    }

    public static implicit operator float(AttackData data) => data.Damage;

    public static AttackData operator +(AttackData data, float value)
    {
        data.Damage += value;
        return data;
    }

    public static AttackData operator -(AttackData data, float value)
    {
        data.Damage -= value;
        return data;
    }

    public static AttackData operator *(AttackData data, float value)
    {
        data.Damage *= value;
        return data;
    }

    public static AttackData operator /(AttackData data, float value)
    {
        data.Damage /= value;
        return data;
    }
}
