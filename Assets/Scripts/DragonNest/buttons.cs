using UnityEngine;
using UnityEngine.UI;

public class buttons : MonoBehaviour
{
    public Sprite spriteOn;   
    public Sprite spriteOff;  

    private Image buttonImage;
    private bool isToggled = false;

    void Start()
    {
        buttonImage = GetComponent<Image>();
        buttonImage.sprite = spriteOn;
    }

    public void ToggleSprite()
    {
        isToggled = !isToggled;
        buttonImage.sprite = isToggled ? spriteOff : spriteOn;
    }

    public void MuteMusic()
    {
        ToggleSprite();
        MusicPlayer.Testing();
    }

    public void SetSFXMute()
    {
        ToggleSprite();
        AudioManager.Instance.SetVolumeSFX(isToggled);
        
    }
}
