using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        SceneSwitcher.SwitchToEgg();
    }

    public void Credits()
    {
        SceneSwitcher.SwitchToCredits();
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
