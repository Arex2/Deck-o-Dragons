using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SceneSwitcherCutout : SceneSwitcherTransition
{
    [CacheComponent]
    [SerializeField] private Image image;

    [Space]
    [SerializeField] private float extraScale;

#if UNITY_EDITOR
    [Space]
    [SerializeField] private bool showEditorImagePreview = false;
#endif

    public RectTransform RectTransform
    {
        get
        {
            if (_rectTransform == null)
            {
                _rectTransform = transform as RectTransform;
            }
            return _rectTransform;
        }
    }
    private RectTransform _rectTransform;

    private float _scale;

    public override void Initialize()
    {
        CalculateCutoutScale(out _);

        // Instantly disappear
        Disappear(0);

        Disable();
    }

    public override void Enable()
    {
        // Toggle image
        image.enabled = true;
    }

    public override void Disable()
    {
        // Toggle image
        image.enabled = false;
    }

    public override void Appear(float duration, Ease ease = Ease.Unset, Action onFinish = null)
    {
        // Kill tweens
        RectTransform.DOKill();

        SceneSwitcher.ScreenOverlay.enabled = true;

        // 0 or below means instant
        if (duration <= 0)
        {
            // Instantly set scale
            RectTransform.localScale = Vector2.zero;
            onFinish?.Invoke();
        }
        else
        {
            // Use DOTween for scaling
            SceneSwitcher.ScreenOverlay.enabled = true;

            RectTransform.DOScale(0, duration).SetEase(ease).onComplete = new TweenCallback(onFinish);
        }
    }

    public override void Disappear(float duration, Ease ease = Ease.Unset, Action onFinish = null)
    {
        // Kill tweens
        RectTransform.DOKill();

        // 0 or below means instant
        if (duration <= 0)
        {
            // Instantly set scale
            RectTransform.localScale = _scale * Vector2.one;
            onFinish?.Invoke();
        }
        else
        {
            // Use DOTween for scaling
            SceneSwitcher.ScreenOverlay.enabled = true;

            RectTransform.DOScale(_scale, duration).SetEase(ease).onComplete = () =>
            {
                SceneSwitcher.ScreenOverlay.enabled = false;

                onFinish?.Invoke();
            };
        }
    }

    public void CalculateCutoutScale(out Vector2 size)
    {
        Sprite transitionSprite = image.sprite;

        float spriteWidth = transitionSprite == null ? RectTransform.rect.width : transitionSprite.texture.width;
        float spriteHeight = transitionSprite == null ? RectTransform.rect.height : transitionSprite.texture.height;

        size = SceneSwitcher.WholeScreenSize;

        float spriteRatio = spriteWidth / spriteHeight;
        float rectRatio = size.x / size.y;

        // Too Tall
        if (spriteRatio > rectRatio)
        {
            float oldHeight = size.y;

            size.y = size.x * (1f / spriteRatio);

            _scale = oldHeight / size.y;
        }
        // Too Wide
        else
        {
            float oldWidth = size.x;

            size.x = size.y * spriteRatio;

            _scale = oldWidth / size.x;
        }

        // Add extra scaling
        _scale += extraScale;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (SceneSwitcher == null)
        {
            return;
        }

        if (image == null)
        {
            return;
        }

        CalculateCutoutScale(out Vector2 size);

        Matrix4x4 startMatrix = Gizmos.matrix;
        Matrix4x4 noScaleMatrix = Matrix4x4.TRS(RectTransform.transform.position, Quaternion.identity, Vector2.one);
        Gizmos.matrix = noScaleMatrix;

        // Draw cube
        Gizmos.color = Color.blue;
        DrawCube(size * _scale);

        // Draw editor image preview
        if (image.sprite != null && showEditorImagePreview)
        {
            Vector2 spriteSize = size * _scale;

            Vector2 rectPos = (Vector2)RectTransform.transform.position - spriteSize / 2;
            Vector2 rectSize = spriteSize;

            // For some reason the image is upside down??? (this fixes it)
            rectPos.y += spriteSize.y;
            rectSize.y *= -1;

            Rect textureRect = new Rect(rectPos, rectSize);
            Gizmos.DrawGUITexture(textureRect, image.sprite.texture);
        }

        Gizmos.matrix = Matrix4x4.TRS(RectTransform.transform.position, Quaternion.identity, RectTransform.transform.localScale);

        // Draw regular SCALED cube
        Gizmos.color = Color.red;
        DrawCube(size);

        Gizmos.matrix = noScaleMatrix;

        // Draw regular no scaling cube
        Gizmos.color = Color.yellow;
        DrawCube(size);

        Gizmos.matrix = startMatrix;
    }

    private void DrawCube(Vector3 size)
    {
        Gizmos.DrawWireCube(Vector3.zero, size);

        Vector3 halfSize = size / 2;
        Gizmos.DrawLine(-halfSize, halfSize);
        halfSize.x *= -1;
        Gizmos.DrawLine(-halfSize, halfSize);
    }
#endif
}
