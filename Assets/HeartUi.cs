using UnityEngine;
using UnityEngine.UI;

public class HeartUI : MonoBehaviour
{
    public Image[] hearts;           // Array to hold references to heart UI images
    public Sprite fullHeartSprite;   // Sprite for a full heart
    public Sprite emptyHeartSprite;  // Sprite for an empty heart
    private int currentHealth = 3;   // Starting health (3 hearts)

    void Start()
    {
        SetHealth(2);
        UpdateHearts();
    }

    public void SetHealth(int health)
    {
        currentHealth = health;
        UpdateHearts();
    }

    private void UpdateHearts()
    {
        // Loop through all the hearts and update based on current health
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
            {
                hearts[i].sprite = fullHeartSprite;  // Set the heart to full
            }
            else
            {
                hearts[i].sprite = emptyHeartSprite; // Set the heart to empty
            }
        }
    }
}
