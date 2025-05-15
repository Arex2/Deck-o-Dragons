using UnityEngine;
using UnityEngine.UI;

public class SceneSwitchButton : MonoBehaviour
{
    [SerializeField] private SceneSwitcher.Scene scene;
    private AudioClip transitionSFX;

    private void Awake()
    {
        transitionSFX = Resources.Load<AudioClip>("Audio/TransitionSFX");
        foreach (Button button in GetComponentsInChildren<Button>(true))
        {
            button.onClick.AddListener(Switch);
        }
    }

    public void Switch()
    {
        AudioManager.Instance.PlayClickSound();
        AudioManager.Instance.PlaySFX(transitionSFX);
        SceneSwitcher.SwitchScene(SceneSwitcher.GetScene(scene));
    }
}
