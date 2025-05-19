using UnityEngine;
using UnityEngine.UI;

public class SceneSwitchButton : MonoBehaviour
{
    [SerializeField] private SceneSwitcher.Scene scene;

    private void Awake()
    {
        foreach (Button button in GetComponentsInChildren<Button>(true))
        {
            button.onClick.AddListener(Switch);
        }
    }

    public void Switch()
    {
        AudioManager.Instance.PlayClickSound();
        SceneSwitcher.SwitchScene(SceneSwitcher.GetScene(scene));
    }
}
