using UnityEngine;
using UnityEngine.UI;

public class BrushingController : MonoBehaviour
{
    public static BrushingController instance;

    public bool isBrushing = false;

    public Button brushButton;
    public Sprite normalSprite;
    public Sprite pressedSprite;

    void Awake()
    {
        instance = this;
    }

    public void ToggleBrushMode()
    {
        isBrushing = !isBrushing;

        if (isBrushing)
        {
            brushButton.GetComponent<Image>().sprite = pressedSprite;
        }
        else
        {
            brushButton.GetComponent<Image>().sprite = normalSprite;
        }
    }

}
