using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TargetSelectorButton : MonoBehaviour
{
    public Target Target { get; set; }
    public TargetSelector TargetSelector { get; set; }

    public bool Activated { get; private set; } = true;
    public bool Visible { get; private set; } = true;

    [CacheComponent]
    [SerializeField] private Button button;
    [CacheComponent]
    [SerializeField] private CanvasGroup canvasGroup;

    [Space]
    [SerializeField] private Image image;

    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = transform as RectTransform;
    }

    private void Update()
    {
        if (!Visible || Target == null)
        {
            return;
        }

        Camera camera = TargetSelector.Camera;

        if (camera == null)
        {
            return;
        }

        Bounds bounds = Target.GetWorldBounds();

        Vector2 min = camera.WorldToScreenPoint(bounds.min);
        Vector2 max = camera.WorldToScreenPoint(bounds.max);

        _rectTransform.position = (min + max) / 2;
        _rectTransform.sizeDelta = (max - min) / TargetSelector.ScaleFactor;
    }

    public void ChooseTarget()
    {
        TargetSelector.ChooseTarget(Target);
    }

    public void Activate(float fadeInTime)
    {
        if (Activated)
        {
            return;
        }

        Activated = true;
        Visible = true;

        image.DOKill();

        if (fadeInTime <= 0)
        {
            canvasGroup.blocksRaycasts = true;

            Color color = image.color;
            color.a = 1;
            image.color = color;
        }
        else
        {
            canvasGroup.blocksRaycasts = false;
            image.DOFade(1, fadeInTime).onComplete = () =>
            {
                canvasGroup.blocksRaycasts = true;
            };
        }
    }

    public void Deactivate(float fadeInTime)
    {
        if (!Activated)
        {
            return;
        }

        Activated = false;

        image.DOKill();
        canvasGroup.blocksRaycasts = false;

        if (fadeInTime <= 0)
        {
            Color color = image.color;
            color.a = 0;
            image.color = color;

            Visible = false;
        }
        else
        {
            Visible = true;
            image.DOFade(0, fadeInTime).onComplete = () =>
            {
                canvasGroup.blocksRaycasts = false;
                Visible = false;
            };
        }
    }
}
