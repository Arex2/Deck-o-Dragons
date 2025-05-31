using UnityEngine;

[SingletonMode(true)]
public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Sources")]
    [SerializeField] AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] AudioClip clickSound;

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    

    public void PlayClickSound()
    {
        PlaySFX(clickSound);
    }

    public void SetVolumeSFX(bool value)
    {

        sfxSource.volume = value ? 0 : 1;
    }
}
