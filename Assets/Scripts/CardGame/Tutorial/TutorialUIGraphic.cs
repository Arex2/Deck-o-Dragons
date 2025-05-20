using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TutorialUIGraphic : TutorialObject
{
    [SerializeField] private bool includeChildren = false;

    private CanvasGroup[] _canvasGroup;
    private Graphic[] _graphics;

    private Dictionary<Graphic, float> _activeAlphas = new();

    private void Awake()
    {
        _canvasGroup = includeChildren ? GetComponentsInChildren<CanvasGroup>(true) : GetComponents<CanvasGroup>();
        _graphics = includeChildren ? GetComponentsInChildren<Graphic>(true) : GetComponents<Graphic>();

        foreach (Graphic graphic in _graphics)
        {
            Color color = graphic.color;

            _activeAlphas[graphic] = color.a;
            color.a = 0;

            graphic.color = color;
        }

        foreach (CanvasGroup canvasGroup in _canvasGroup)
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public override void Enable() => Toggle(true);
    public override void Disable() => Toggle(false);

    private void Toggle(bool enable)
    {
        foreach (Graphic graphic in _graphics)
        {
            graphic.DOKill();

            if (enable)
            {
                if (!_activeAlphas.TryGetValue(graphic, out float graphicAlpha))
                {
                    graphicAlpha = 1;
                }

                graphic.DOFade(graphicAlpha, 0.5f);
            }
            else
            {
                graphic.DOFade(0, 0.5f);
            }
        }

        float alpha = enable ? 1 : 0;

        foreach (CanvasGroup canvasGroup in _canvasGroup)
        {
            canvasGroup.DOKill();
            canvasGroup.DOFade(alpha, 0.5f);
            canvasGroup.blocksRaycasts = enable;
        }
    }
}
