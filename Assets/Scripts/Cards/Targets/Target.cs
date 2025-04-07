using UnityEngine;

/// <summary>
/// An object that can be targetted by a <see cref="Card"/> or enemy.
/// </summary>
// Script by Ruben
public abstract class Target : MonoBehaviour
{
    public abstract Team Team { get; }
    public float MaxHP => maxHp;
    public float HP => hp;

    public bool IsLeader => isLeader;

    [SerializeField] private float maxHp;
    protected float hp;

    [SerializeField] private bool isLeader;

    private void OnEnable()
    {
        TargetManager.AddCardTarget(this);
    }

    private void OnDisable()
    {
        TargetManager.RemoveCardTarget(this);
    }

    protected virtual void Awake()
    {
        hp = maxHp;
    }

    protected virtual void Start()
    {

    }

    public virtual void Hurt(float amount)
    {
        hp -= amount;

        if (hp < 0)
        {
            hp = 0;
        }

        //Debug.Log(name + " has taken " + amount + " damage");
        UpdateHP();
    }

    public virtual void Heal(float amount)
    {
        hp += amount;

        if (hp > maxHp)
        {
            hp = maxHp;
        }

        //Debug.Log(name + " has healed " + amount + " HP");
        UpdateHP();
    }

    protected virtual void UpdateHP()
    {

    }

    public abstract Bounds GetWorldBounds();
}
