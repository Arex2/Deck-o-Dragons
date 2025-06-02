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
            if(clip == clickSound)
            {
                sfxSource.PlayOneShot(clip, 0.5f); ;
            }
            else
            {
                sfxSource.PlayOneShot(clip);
            }
        }
    }

    public void PlayIncrVolumeSFX(AudioClip clip)
    {
        if(clip != null)
        {
            sfxSource.PlayOneShot(clip, 1.5f);
        }
    }

    public void PlayDecrVolumeSFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, 0.5f);
        }
    }

    public void PlayClickSound()
    {
        PlaySFX(clickSound);
    }
}
