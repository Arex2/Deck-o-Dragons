using UnityEngine;

public class PanelToggle : MonoBehaviour
{
    public GameObject buttonPanel;

    public void TogglePanel()
    {
        if (buttonPanel != null)
        {
            buttonPanel.SetActive(!buttonPanel.activeSelf);
            AudioManager.Instance.PlayClickSound();
        }
    }
}
