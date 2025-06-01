using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PanelToggle : MonoBehaviour
{
    public Transform buttonPanel;
    public CanvasGroup canvasGroup;
    public Image background;

    [Space]
    public bool toggled;

    private float backgroundAlpha;

    private void Awake()
    {
        if (buttonPanel != null)
        {
            buttonPanel.localScale = toggled ? Vector3.one : Vector3.zero;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = toggled;
        }

        if (background != null)
        {
            Color color = background.color;
            backgroundAlpha = color.a;
            color.a = toggled ? backgroundAlpha : 0;
            background.color = color;
        }
    }

    public void TogglePanel()
    {
        AudioManager.Instance.PlayClickSound();

        SetToggle(!toggled);
    }

    public void SetToggle(bool toggle)
    {
        toggled = toggle;

        if (buttonPanel != null)
        {
            buttonPanel.DOKill();
            buttonPanel.DOScale(toggled ? Vector3.one : Vector3.zero, toggled ? 0.5f : 0.35f).SetEase(toggled ? Ease.OutBack : Ease.InBack);
        }

        if (canvasGroup != null)
        {
            canvasGroup.DOKill();
            canvasGroup.blocksRaycasts = toggled;
        }

        if (background != null)
        {
            background.DOKill();
            background.DOFade(toggled ? backgroundAlpha : 0, 0.5f);
        }
    }
}
