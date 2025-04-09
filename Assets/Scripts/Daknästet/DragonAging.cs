using UnityEngine;

public class DragonController : MonoBehaviour
{
    [SerializeField] private Sprite[] dragonSprites;
    private int currentIndex = 0;


    [Header("Reset Settings")]
    [SerializeField] private GameObject eggPrefab;
    [SerializeField] private SpriteRenderer spriteRenderer;

    void Start()
    {
        if (dragonSprites.Length > 0)
        {
            spriteRenderer.sprite = dragonSprites[0];
        }
    }
    public void NextDragon()
    {
        if (dragonSprites.Length == 0) return;

        currentIndex = (currentIndex + 1) % dragonSprites.Length;
        spriteRenderer.sprite = dragonSprites[currentIndex];
    }
    public void ResetToEgg()
    {
        Instantiate(eggPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        Destroy(gameObject);
    }
}
