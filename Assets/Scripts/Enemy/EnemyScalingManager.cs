using UnityEngine;

public class EnemyScalingManager : MonoBehaviour
{
    public static EnemyScalingManager Instance { get; private set; }

    [Header("Scaling")]
    [SerializeField] private int baseHealth = 20;
    [SerializeField] private int baseMana = 5;

    [SerializeField] private int healthIncrease = 10;
    [SerializeField] private int manaIncrease = 3;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int GetScaledHealth()
    {
        return baseHealth + healthIncrease * ProgressManager.currentLevel;
    }

    public int GetScaledMana()
    {
        return baseMana + manaIncrease * ProgressManager.currentLevel;
    }
}
