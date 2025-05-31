using UnityEngine;
using UnityEngine.UI;

public class AudioMuteButton : MonoBehaviour
{
    public Sprite spriteOn;   
    public Sprite spriteOff;

    public bool muteMusic;

    private Image buttonImage;
    private bool isToggled = false;

    void Start()
    {
        buttonImage = GetComponent<Image>();
        buttonImage.sprite = spriteOn;

        if ((muteMusic && SaveManager.MusicMuted) || (!muteMusic && SaveManager.SFXMuted))
        {
            ToggleSprite();
        }
    }

    public void ToggleSprite()
    {
        isToggled = !isToggled;
        buttonImage.sprite = isToggled ? spriteOff : spriteOn;
    }

    public void Mute()
    {
        ToggleSprite();

        if (muteMusic)
        {
            SaveManager.MusicMuted = !SaveManager.MusicMuted;
        }
        else
        {
            SaveManager.SFXMuted = !SaveManager.SFXMuted;
        }
    }
}
