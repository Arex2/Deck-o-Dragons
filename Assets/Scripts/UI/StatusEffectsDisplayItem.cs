using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class StatusEffectsDisplayItem : MonoBehaviour
{
    private RectTransform _rectTransform;

    [CacheComponent]
    [SerializeField] private Image image;

    [CacheComponent]
    [SerializeField] private TMP_Text textComponent;

    [CacheComponent]
    [SerializeField] private LayoutElement layoutElement;

    [Space]
    [SerializeField] private RectTransform moveablePanel;

    private Vector2 _currentPosition;
    private Vector2 _movingTowardsPosition;
    private Vector2 _targetPosition;
    private Tween _moveTween;

    [SerializeField] private float offscreenYOffset;

    private float _yOffset;
    private Tween _yOffsetTween;

    private Vector2 _currentSize;
    private Vector2 _targetSize;
    private Tween _sizeTween;

    public StatusEffectData Data { get; private set; }

    private bool _active = true;

    private void Awake()
    {
        _rectTransform = transform as RectTransform;
    }

    private void Update()
    {
        if (_active)
        {
            _targetPosition = _rectTransform.anchoredPosition;
        }

        if (_movingTowardsPosition != _targetPosition)
        {
            _movingTowardsPosition = _targetPosition;

            _moveTween?.Kill();

            _moveTween = DOTween.To(() => _currentPosition, (value) => _currentPosition = value, _targetPosition, 0.5f);
        }

        Vector2 moveablePanelNewPos = _currentPosition + Vector2.up * _yOffset;

        if (moveablePanel.anchoredPosition != moveablePanelNewPos)
        {
            moveablePanel.anchoredPosition = moveablePanelNewPos;
        }

        if (_targetSize != _rectTransform.sizeDelta)
        {
            _targetSize = _rectTransform.sizeDelta;

            _sizeTween?.Kill();

            _sizeTween = DOTween.To(() => _currentSize, (value) => _currentSize = value, _targetSize, 0.5f);
        }

        if (moveablePanel.sizeDelta != _currentSize)
        {
            moveablePanel.sizeDelta = _currentSize;
        }
    }

    public void Teleport()
    {
        _moveTween?.Kill();
        _sizeTween?.Kill();

        _targetPosition = _rectTransform.anchoredPosition;
        _movingTowardsPosition = _targetPosition;
        _currentPosition = _targetPosition;

        _targetSize = _rectTransform.sizeDelta;
        _currentSize = _targetSize;
    }

    public void Appear(bool instant = false, TweenCallback onFinish = null) => SetActive(true, instant, onFinish);
    public void Disappear(bool instant = false, TweenCallback onFinish = null) => SetActive(false, instant, onFinish);

    public void SetActive(bool active, bool instant = false, TweenCallback onFinish = null)
    {
        layoutElement.ignoreLayout = !active;

        _active = active;

        _yOffsetTween?.Kill();

        float targetYOffset = active ? 0 : offscreenYOffset;

        if (instant)
        {
            _yOffset = targetYOffset;

            onFinish?.Invoke();
            return;
        }

        _yOffsetTween = DOTween.To(() => _yOffset, (value) => _yOffset = value, targetYOffset, 0.5f);
        _yOffsetTween.onComplete = onFinish;
    }

    public void SetInactiveParent(Transform inactiveParent)
    {
        moveablePanel.SetParent(inactiveParent);
    }

    public void SetData(StatusEffectData statusEffectData)
    {
        Data = statusEffectData;

        image.sprite = statusEffectData.StatusEffect.Icon;
        textComponent.text = statusEffectData.Duration.ToString();
    }
}
