using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[SingletonMode(true)]
public class MusicManager : Singleton<MusicManager>
{
    [SerializeField] private SongEntry[] data;

    private Dictionary<int, Song> _songs = new();

    private Song _currentSong;
    private bool _pendingSongPlay;

    private void Start()
    {
        foreach (SongEntry entry in data)
        {
            foreach (SceneSwitcher.Scene scene in entry.Scenes)
            {
                SceneReference sceneReference = SceneSwitcher.GetScene(scene);

                _songs[sceneReference.BuildIndex] = entry.Song;
            }
        }

        UpdateSong(SceneSwitcher.CurrentSceneBuildIndex);

        SceneSwitcher.OnSwitchScene += UpdateSong;
        SceneManager.sceneLoaded += OnSceneLoaded;

        _pendingSongPlay = false;
        PlaySong(_currentSong);
    }

    private void UpdateSong(int sceneIndex)
    {
        _songs.TryGetValue(sceneIndex, out Song song);

        if (_currentSong == song)
        {
            return;
        }

        if (_currentSong != null)
        {
            StartCoroutine(StopSong(_currentSong));
        }

        _currentSong = song;

        _pendingSongPlay = true;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (!_pendingSongPlay)
        {
            return;
        }

        _pendingSongPlay = false;

        PlaySong(_currentSong);
    }

    public void PlaySong(Song song)
    {
        if (song == null)
        {
            return;
        }

        if (song.Loaded)
        {
            InstantPlay(song);
        }
        else
        {
            StartCoroutine(PlaySongAfterLoad(song));
        }
    }

    private IEnumerator PlaySongAfterLoad(Song song)
    {
        song.Load();

        yield return new WaitUntil(() => song.Loaded);

        InstantPlay(song);
    }

    private void InstantPlay(Song song)
    {
        MusicPlayer.PlaySong(song);
    }

    private IEnumerator StopSong(Song song)
    {
        MusicPlayer.StopSong(song, 0.5f);

        yield return new WaitForSecondsRealtime(0.5f);

        song.Unload();
    }

    [Serializable]
    private class SongEntry
    {
        public Song Song => song;
        public SceneSwitcher.Scene[] Scenes => scenes;

        [SerializeField] private Song song;
        [SerializeField] private SceneSwitcher.Scene[] scenes;
    }
}
