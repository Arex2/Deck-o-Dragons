using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SceneSwitcherCutout : MonoBehaviour
{
    [CacheComponent(CacheMethod.InParent)]
    [SerializeField] private SceneSwitcher sceneSwitcher;
    [CacheComponent]
    [SerializeField] private Image image;

    [Space]
    [SerializeField] private float extraScale;
    [Space]
    [SerializeField] private SceneSwitcher.Scene[] scenes;

#if UNITY_EDITOR
    [Space]
    [SerializeField] private bool showEditorImagePreview = false;
#endif

    public SceneSwitcher.Scene[] Scenes => scenes;

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

    public bool ImageEnabled
    {
        get => image.enabled;
        set => image.enabled = value;
    }

    private float _scale;

    public void SetToScale()
    {
        RectTransform.localScale = (_scale + extraScale) * Vector3.one;
    }

    public void CalculateCutoutScale() => CalculateCutoutScale(out _);

    public void CalculateCutoutScale(out Vector2 size)
    {
        Sprite transitionSprite = image.sprite;

        float spriteWidth = transitionSprite == null ? RectTransform.rect.width : transitionSprite.texture.width;
        float spriteHeight = transitionSprite == null ? RectTransform.rect.height : transitionSprite.texture.height;

        size = sceneSwitcher.WholeScreenSize;

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
    }

    public void DoTransitionOut(float duration, Ease outEase, TweenCallback onComplete = null)
    {
        RectTransform.DOKill();
        Tween tween = RectTransform.DOScale(_scale + extraScale, duration).SetEase(outEase);

        if (onComplete != null)
        {
            tween.onComplete = onComplete;
        }
    }

    public void DoTransitionIn(float duration, Ease inEase, TweenCallback onComplete = null)
    {
        RectTransform.DOKill();
        Tween tween = RectTransform.DOScale(0, duration).SetEase(inEase);

        if (onComplete != null)
        {
            tween.onComplete = onComplete;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        DrawGizmos();
    }

    private void DrawGizmos()
    {
        if (sceneSwitcher == null)
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

        // Draw scaled cube
        Gizmos.color = Color.blue;
        DrawCube(size * (_scale + extraScale));

        if (image.sprite != null && showEditorImagePreview)
        {
            Vector2 spriteSize = size * (_scale + extraScale);

            Vector2 rectPos = (Vector2)RectTransform.transform.position - spriteSize / 2;
            Vector2 rectSize = spriteSize;
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

        // Draw regular cube
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
