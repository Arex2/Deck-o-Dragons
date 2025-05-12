using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTet : MonoBehaviour
{
    [SerializeField] private Song song;

    public void DefaultStyle() => MusicPlayer.SetSongStyle(song, "default");
    public void SetStyle(string style) => MusicPlayer.SetSongStyle(song, style);
}
