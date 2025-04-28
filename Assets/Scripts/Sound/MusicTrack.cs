using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class MusicTrack : MonoBehaviour
{
    public Song Song => _layer.Song;
    public SongLayer Layer => _layer;

    public ObjectPool<MusicTrack> Pool { get; set; }

    public bool IsPlaying => source.isPlaying;

    public float Volume => _sourceVolume * _layerVolume;
    public float SourceVolume
    {
        get => _sourceVolume;
        set
        {
            _sourceVolume = value;
            UpdateVolume();
        }
    }

    private float _sourceVolume;
    public float LayerVolume
    {
        get => _layerVolume;
        set
        {
            _layerVolume = value;
            UpdateVolume();
        }
    }

    public float Time
    {
        get => source.time;
        set => source.time = value;
    }

    private void UpdateVolume()
    {
        source.volume = Volume;
    }

    private float _layerVolume;

    [CacheComponent]
    [SerializeField] private AudioSource source;

    private SongLayer _layer;

    public void SetSongLayer(SongLayer layer)
    {
        _layer = layer;
    }

    public void KillAllTweens()
    {
        source.DOKill();
        this.DOKill();
    }

    public void Play(float fadeTime)
    {
        source.DOKill();

        if (fadeTime <= 0)
        {
            SourceVolume = 1;
        }
        else
        {
            TweenSourceVolume(1, fadeTime);
        }

        if (source.clip != _layer.Loop)
        {
            source.clip = _layer.Loop;

            if (_layer.Intro != null)
            {
                source.PlayOneShot(_layer.Intro);
                source.PlayScheduled(AudioSettings.dspTime + ((double)_layer.Intro.samples / (double)_layer.Intro.frequency));
            }
            else
            {
                source.Play();
            }
        }
    }

    public void Stop(float fadeTime)
    {
        source.DOKill();

        if (fadeTime <= 0)
        {
            SourceVolume = 0;
            Release();
        }
        else
        {
            TweenSourceVolume(0, fadeTime).onComplete = Release;
        }
    }

    public void LayerFadeIn(float fadeTime)
    {
        this.DOKill();

        if (fadeTime <= 0)
        {
            LayerVolume = 1;
        }
        else
        {
            TweenLayerVolume(1, fadeTime);
        }
    }

    public void LayerFadeOut(float fadeTime)
    {
        this.DOKill();

        if (fadeTime <= 0)
        {
            LayerVolume = 0;
        }
        else
        {
            TweenLayerVolume(0, fadeTime);
        }
    }

    public Tween TweenSourceVolume(float endValue, float duration)
    {
        Tween tween = DOTween.To(() => SourceVolume, (val) => SourceVolume = val, endValue, duration).SetUpdate(true);
        tween.SetTarget(source);

        return tween;
    }

    public Tween TweenLayerVolume(float endValue, float duration)
    {
        Tween tween = DOTween.To(() => LayerVolume, (val) => LayerVolume = val, endValue, duration).SetUpdate(true);
        tween.SetTarget(this);

        return tween;
    }

    private void Release()
    {
        source.Stop();
        source.clip = null;

        Pool.Release(this);
    }
}