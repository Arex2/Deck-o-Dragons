using UnityEngine;

public class DragonController : MonoBehaviour
{
    [SerializeField] private Sprite[] dragonSprites;
    [SerializeField] private GameObject eggPrefab;

    private SpriteRenderer spriteRenderer;
    private int currentIndex = 0;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (dragonSprites.Length > 0 && spriteRenderer != null)
        {
            spriteRenderer.sprite = dragonSprites[0];
        }
    }

    void OnMouseDown()
    {
        NextDragon();
    }

    public void NextDragon()
    {
        if (dragonSprites.Length == 0 || spriteRenderer == null) return;

        currentIndex = (currentIndex + 1) % dragonSprites.Length;
        spriteRenderer.sprite = dragonSprites[currentIndex];
    }
}
