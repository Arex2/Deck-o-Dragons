using UnityEngine;

public class SongLayer : ScriptableObject
{
    public Song Song => song;
    public AudioClip Intro => intro;
    public AudioClip Loop => loop;

    public bool Loaded
    {
        get
        {
            return (intro == null || intro.loadState == AudioDataLoadState.Loaded) && (loop == null || loop.loadState == AudioDataLoadState.Loaded);
        }
    }

    [SerializeField] private Song song;

    [SerializeField] private AudioClip intro;
    [SerializeField] private AudioClip loop;

    public void Load()
    {
        if (intro != null)
        {
            intro.LoadAudioData();
        }
        if (loop != null)
        {
            loop.LoadAudioData();
        }
    }

    public void Unload()
    {
        if (intro != null)
        {
            intro.UnloadAudioData();
        }
        if (loop != null)
        {
            loop.UnloadAudioData();
        }
    }
}