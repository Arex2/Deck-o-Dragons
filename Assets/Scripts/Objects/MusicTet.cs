using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTet : MonoBehaviour
{
    [SerializeField] private Song song;

    public void DefaultStyle() => MusicPlayer.SetSongStyle(song, "default");
    public void NoDrumsStyle() => MusicPlayer.SetSongStyle(song, "no drums");

    private void Start()
    {
        OpenKeyboard();
    }

    private TouchScreenKeyboard keyboard;

    public void OpenKeyboard()
    {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
    }

    private void Update()
    {
        if (keyboard.status == TouchScreenKeyboard.Status.Done)
        {
            Debug.Log("Dragon name is " + keyboard.text);
        }
    }
}
