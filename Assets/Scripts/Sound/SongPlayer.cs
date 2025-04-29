using System.Collections;
using UnityEngine;

public class SongPlayer : MonoBehaviour
{
    [SerializeField] private bool playOnStart = true;

    [Space]
    [SerializeField] private Song song;
    [SerializeField] private float fadeTime = 0.5f;
    [SerializeField] private string style;

    private void Start()
    {
        if (!song.Loaded)
        {
            song.Load();
        }

        if (playOnStart)
        {
            PlaySong();
        }
    }

    public void PlaySong()
    {
        StartCoroutine(PlaySongAfterLoad());
    }

    private IEnumerator PlaySongAfterLoad()
    {
        yield return new WaitUntil(() => song.Loaded);

        InstantPlay();
    }

    private void InstantPlay()
    {
        MusicPlayer.PlaySong(song, fadeTime, true, style);
    }
}
