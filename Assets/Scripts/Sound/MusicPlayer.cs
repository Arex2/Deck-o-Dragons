using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System;

[SingletonMode(true)]
public class MusicPlayer : Singleton<MusicPlayer>
{
    [SerializeField] private MusicTrack templateTrack;

    [Space]
    [SerializeField] private float defaultTrackFadeTime = 0.5f;

    private static HashSet<Song> _songsPlaying = new HashSet<Song>();
    private static HashSet<MusicTrack> _activeMusicTracks = new HashSet<MusicTrack>();
    private static int _createdMusicTracks;
    private static bool muted = false;

    private static ObjectPool<MusicTrack> _musicTrackPool;

    protected override void Awake()
    {
        base.Awake();

        _musicTrackPool = new ObjectPool<MusicTrack>(CreateMusicTrack,
            (track) => _activeMusicTracks.Add(track),
            (track) => _activeMusicTracks.Remove(track)
            );
    }

    private MusicTrack CreateMusicTrack()
    {
        MusicTrack track = Instantiate(templateTrack, transform);

        track.Pool = _musicTrackPool;
        track.gameObject.SetActive(true);

        _createdMusicTracks++;
        track.name = "Track " + _createdMusicTracks;

        return track;
    }

    public static bool IsPlaying(Song song)
    {
        return _songsPlaying.Contains(song);
    }

    public static void Testing()
    {
        muted = !muted;

        
            foreach (MusicTrack track in _activeMusicTracks)
            {
            track.SourceVolume = muted ? 0 : 1;
            }
        
    }

    public static void PlaySong(Song song, float fadeTime = 0, bool instantIfNotAlreadyPlaying = true, string style = null)
    {
        _songsPlaying.Add(song);

        bool tracksAlreadyPlayingSong = false;
        foreach (MusicTrack track in _activeMusicTracks)
        {
            if (track.IsPlaying && track.Song == song)
            {
                track.Play(fadeTime);
                tracksAlreadyPlayingSong = true;
            }
        }

        if (tracksAlreadyPlayingSong)
        {
            if (!string.IsNullOrEmpty(style))
            {
                SetSongStyle(song, style, fadeTime, fadeTime);
            }
            return;
        }

        foreach (SongLayer layer in song.Layers)
        {
            MusicTrack track = _musicTrackPool.Get();

            track.KillAllTweens();

            track.SetSongLayer(layer);
            track.Play(instantIfNotAlreadyPlaying ? 0 : fadeTime);
            track.SourceVolume = muted ? 0 : 1;
            track.LayerVolume = 0;
        }

        SetSongStyle(song, string.IsNullOrEmpty(style) ? "default" : style, 0, 0);
    }

    public static void SetSongStyle(Song song, string style, float? fadeInTime = null, float? fadeOutTime = null)
    {
        if (!_songsPlaying.Contains(song))
        {
            return;
        }

        List<SongLayer> layers = song.GetStyleLayers(style);

        if (layers == null)
        {
            return;
        }

        if (!fadeInTime.HasValue)
        {
            fadeInTime = Instance.defaultTrackFadeTime;
        }

        if (!fadeOutTime.HasValue)
        {
            fadeOutTime = Instance.defaultTrackFadeTime;
        }

        foreach (MusicTrack track in _activeMusicTracks)
        {
            if (track.Song != song)
            {
                continue;
            }

            if (layers.Contains(track.Layer))
            {
                track.LayerFadeIn(fadeInTime.Value);
            }
            else
            {
                track.LayerFadeOut(fadeOutTime.Value);
            }
        }
    }

    public static void StopSong(Song song, float fadeTime)
    {
        if (!_songsPlaying.Remove(song))
        {
            return;
        }

        foreach (MusicTrack track in _activeMusicTracks)
        {
            if (track.IsPlaying && track.Song == song)
            {
                track.Stop(fadeTime);
            }
        }
    }

    public static void Resync(Song song)
    {
        if (!IsPlaying(song))
        {
            return;
        }

        float time = -1;

        SongLayer firstLayer = song.Layers[0];

        List<MusicTrack> tracks = new List<MusicTrack>();

        List<float> times = new List<float>();

        foreach (MusicTrack track in _activeMusicTracks)
        {
            if (!track.IsPlaying || track.Song != song)
            {
                continue;
            }

            tracks.Add(track);

            times.Add(track.Time);

            if (track.Layer == firstLayer)
            {
                time = track.Time;
            }
        }

        if (time < 0 || times.Distinct().Count() <= 1)
        {
            return;
        }

        foreach (MusicTrack track in tracks)
        {
            track.Time = time;
        }
    }
}