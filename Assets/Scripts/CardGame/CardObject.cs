using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class CardObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
{
    public bool WaitingForCardsToAffect => Card.WaitingForCardsToAffect;
    public List<CardObject> CardsToAffect => Card.CardsToAffect;

    public bool Selectable { get; private set; }

    /// <summary>
    /// Level of the <see cref="card"/>.
    /// </summary>
    public int Level
    {
        get => level;
        set
        {
            if (level == value)
            {
                return;
            }

            level = value;

            //Card.UpdateDescription();
            cardVisuals.UpdateCardLook();
        }
    }

    private int level;

    public float TimeCreated { get; set; }

    public int Cost
    {
        get => Mathf.Max(Card.Cost + CostOffset, 0);
        set
        {
            value -= Card.Cost;

            CostOffset = value;
        }
    }

    public int CostOffset { get; set; }

    public float StartYPos { get; set; }

    public CardHand CardHand { get; set; }
    public Canvas Canvas => cardVisuals.Canvas;
    public CardVisuals CardVisuals => cardVisuals;

    public Vector2 SizeDelta => rectTransform.sizeDelta;
    public float Width => SizeDelta.x;
    public float Height => SizeDelta.y;

    [CacheComponent]
    [SerializeField] private RectTransform rectTransform;

    [CacheComponent]
    [SerializeField] private CardVisuals cardVisuals;

    [Space]
    [SerializeField] private Target user;

    [Space]
    [SerializeField] private float pointerMoveRadius = 0.15f;

    public Action<CardObject> OnCardPressed { get; set; }

    public Card Card { get; private set; }
    public TagData TagData => _tagData;

    private CardData _cardData;
    private TagData _tagData;

    private bool _pressed;

    private bool _isDestroyingSelf;

    private bool _doingDissolveAnimation;

    private Coroutine _coroutine;

    // Tweening
    private Tween[] _transformInHandTweens = new Tween[3];
    private Vector2 _posInHand;
    private float _rotInHand;
    private float _scaleInHand;

    private float _amountInHand = 0;

    private Vector2 _startPos;
    private float _startRot;
    private float _startScale;

    private Vector2 _currentPos;
    private float _currentRot;
    private float _currentScale;

    private Tween[] _offsetTweens = new Tween[2];
    private Vector2 _offset;

    private bool _discardOnFinish;

    private void Awake()
    {
        _startPos = transform.localPosition;
        _startRot = transform.localEulerAngles.z;
        _startScale = (transform.localScale.x + transform.localScale.y) / 2f;

        _currentPos = _startPos;
        _currentRot = _startRot;
        _currentScale = _startScale;
    }

    public void Initialize(Card card)
    {
        SetCard(card);

        Spawn();
    }

    private void Update()
    {
        Vector2 pos;
        float rot;
        float scale;

        if (_amountInHand >= 1)
        {
            pos = _posInHand;
            rot = _rotInHand;
            scale = _scaleInHand;
        }
        else
        {
            pos = Vector2.Lerp(_startPos, _posInHand, _amountInHand);
            rot = Mathf.Lerp(_startRot, _rotInHand, _amountInHand);
            scale = Mathf.Lerp(_startScale, _scaleInHand, _amountInHand);
        }

        pos += _offset;

        /*
        // For some reason, positions become either NaN or Infinity so this should fix that (?)
        void FixValue(ref float value, float fixValue)
        {
            // "I'm normal" said the float
            if (!float.IsNaN(value) && !float.IsInfinity(value))
            {
                // And it was correct
                return;
            }

            // But it lied, for it was evil float all along
            // Exterminate evil floats
            value = fixValue;
        }

        FixValue(ref pos.x, _currentPos.x);
        FixValue(ref pos.y, _currentPos.y);

        FixValue(ref rot, _currentRot);
        FixValue(ref scale, _currentScale);
        */

        if (_currentPos != pos)
        {
            _currentPos = pos;
            transform.localPosition = pos;
        }

        if (_currentRot != rot)
        {
            _currentRot = rot;
            transform.localRotation = Quaternion.AngleAxis(rot, Vector3.forward);
        }

        if (_currentScale != scale)
        {
            _currentScale = scale;
            transform.localScale = scale * Vector3.one;
        }
    }

    public string GetDescription()
    {
        return Card.GetDescription(Level, _cardData);
    }

    public void UpdateCardLook()
    {
        cardVisuals.UpdateCardLook();
    }

    //Method to update mana cost text when mana affecting cards have been played
    public void UpdateCostLook()
    {
        cardVisuals.UpdateCostText();
    }

    public void SetCard(Card card)
    {
        Card = card;
        cardVisuals.Card = card;

        _tagData = card.Tags;
    }

    public void Play(bool discard = true)
    {
        if (_coroutine != null)
        {
            return;
        }

        _discardOnFinish = discard;

        Card.VFXSpawnOrigin = CardHand.cardPlayPosition;
        _coroutine = StartCoroutine(Card.Play(user, _cardData, _tagData, Level, OnFinishPlayingCard));
    }

    public void Cancel()
    {
        if (_coroutine == null)
        {
            return;
        }

        Card.Cancel();
        _coroutine = null;

        StopCoroutine(_coroutine);
        OnFinishPlayingCard();
    }

    public void Dissolve()
    {
        if (_doingDissolveAnimation)
        {
            return;
        }

        _doingDissolveAnimation = true;
        cardVisuals.Dissolve.TweenDissolveAmount(1, 1).onComplete = () => _doingDissolveAnimation = false;
    }

    public void Spawn()
    {
        _coroutine = null;
        _doingDissolveAnimation = false;

        cardVisuals.Dissolve.DOKill();
        cardVisuals.Dissolve.DissolveAmount = 0;
        cardVisuals.CanvasGroup.blocksRaycasts = true;

        cardVisuals.RandomizeDissolve();

        _currentPos = _startPos;
        _currentRot = _startRot;
        _currentScale = _startScale;

        transform.localPosition = _startPos;

        _amountInHand = 0;
        DOTween.To(() => _amountInHand, (value) => _amountInHand = value, 1, 0.5f).SetEase(Ease.OutSine);

        UpdateCardLook();
    }

    private void OnFinishPlayingCard()
    {
        CardHand.OnFinishPlayingCard();

        if (_discardOnFinish)
        {
            Destroy();

            CardHand.DiscardCard(this);
        }
        else
        {
            Spawn();
            CardHand.AddCardObjToHand(this);
        }
    }

    public bool OnKeptAfterDiscard()
    {
        if (HasTag(CardManager.SlipperyTag))
        {
            return false;
        }

        bool updateCardLook = false;

        if (DecreaseTagPotency(CardManager.BindingTag))
        {
            updateCardLook = true;
        }

        if (DecreaseTagPotency(CardManager.UnplayableTag))
        {
            updateCardLook = true;
        }

        if (DecreaseTagPotency(CardManager.VanishingTag))
        {
            if (!HasTag(CardManager.VanishingTag))
            {
                return false;
            }

            updateCardLook = true;
        }

        if (TryGetTagPotency(CardManager.ThornsTag, out float thornsPotency))
        {
            CardHand.GameBehaviour.Hurt(new(thornsPotency, false, true));
        }

        if (updateCardLook)
        {
            UpdateCardLook();
        }

        return true;
    }

    public bool DecreaseTagPotency(CardTag tag, float decreaseFactor = 1, bool removeOnZero = true)
    {
        if (TryGetTagPotency(tag, out float potency) && potency > 0)
        {
            potency -= decreaseFactor;

            if (potency <= 0 && removeOnZero)
            {
                RemoveTag(tag);
            }
            else
            {
                SetTagPotency(tag, potency);
            }

            return true;
        }

        return false;
    }

    public void SetTag(CardTag tag) => _tagData.SetTag(tag);

    public void SetTagPotency(CardTag tag, float potency) => _tagData.SetPotency(tag, potency);

    public void RemoveTag(CardTag tag) => _tagData.RemoveTag(tag);

    public bool HasTag(CardTag tag) => _tagData.HasTag(tag);

    public float GetTagPotency(CardTag tag) => _tagData[tag];

    public bool TryGetTagPotency(CardTag tag, out float potency) => _tagData.TryGet(tag, out potency);

    #region Card Data
    public void SetCardData<T>(string key, T value) => _cardData.SetCardData(key, value);

    public T GetCardData<T>(string key) => _cardData.GetCardData<T>(key);

    public T GetCardData<T>(string key, T defaultValue) => _cardData.GetCardData(key, defaultValue);

    public bool HasCardData<T>(string key) => _cardData.HasCardData<T>(key);

    public bool TryGetCardData<T>(string key, out T value) => _cardData.TryGetCardData(key, out value);

    public bool TryGetCardData<T>(string key, out T value, T defaultValue) => _cardData.TryGetCardData(key, out value, defaultValue);
    #endregion

    #region Tweening
    public void TweenTransformInHand(Vector2 position, float rotation, float scale, float posDuration, float rotDuration, float scaleDuration, Ease ease = Ease.Unset)
    {
        KillTweens(_transformInHandTweens);

        if (posDuration <= 0)
        {
            _posInHand = position;
        }
        else
        {
            _transformInHandTweens[0] = DOTween.To(() => _posInHand, (value) => _posInHand = value, position, posDuration).SetEase(ease);
        }

        if (rotDuration <= 0)
        {
            _rotInHand = rotation;
        }
        else
        {
            _transformInHandTweens[1] = DOTween.To(() => _rotInHand, (value) => _rotInHand = value, rotation, rotDuration).SetEase(ease);
        }

        if (scaleDuration <= 0)
        {
            _scaleInHand = scale;
        }
        else
        {
            _transformInHandTweens[2] = DOTween.To(() => _scaleInHand, (value) => _scaleInHand = value, scale, scaleDuration).SetEase(ease);
        }
    }

    public void SetTransformInHand(Vector2 position, float rotation, float scale)
    {
        KillTweens(_transformInHandTweens);

        _posInHand = position;
        _rotInHand = rotation;
        _scaleInHand = scale;
    }

    private void KillTweens(Tween[] tweens)
    {
        foreach (Tween tween in tweens)
        {
            tween?.Kill();
        }
    }

    public void TweenOffset(Vector2 offset, float duration)
    {
        TweenOffsetX(offset.x, duration);
        TweenOffsetY(offset.y, duration);
    }

    public Tween TweenOffsetX(float offset, float duration)
    {
        return TweenOffsetIndex(0, offset, duration);
    }

    public Tween TweenOffsetY(float offset, float duration)
    {
        return TweenOffsetIndex(1, offset, duration);
    }

    private Tween TweenOffsetIndex(int index, float offset, float duration)
    {
        _offsetTweens[index]?.Kill();

        if (duration <= 0)
        {
            _offset[index] = offset;

            return null;
        }
        else
        {
            Tween tween = DOTween.To(() => _offset[index], (value) => _offset[index] = value, offset, duration);
            _offsetTweens[index] = tween;

            return tween;
        }
    }

    #endregion

    public void OnPointerDown(PointerEventData eventData)
    {
        _pressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_pressed)
        {
            OnCardPressed?.Invoke(this);
        }

        _pressed = false;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (!_pressed)
            return;

        float dist = Vector2.Distance(eventData.pointerPressRaycast.worldPosition, eventData.pointerCurrentRaycast.worldPosition);

        if (dist < pointerMoveRadius)
            return;

        _pressed = false;
    }

    public void ToggleSelectable(bool selectable)
    {
        Selectable = selectable;

        cardVisuals.ToggleDisabledText(!selectable);
    }

    public void SetDisabledText(string text) => cardVisuals.SetDisabledText(text);

    public void ToggleCheckmark(bool visible)
    {
        cardVisuals.ToggleCheckmark(visible);
    }

    public void ToggleDarkOverlay(bool visible)
    {
        if (visible)
        {
            cardVisuals.SetOverlayColor(Color.black);
            cardVisuals.FadeOverlay(0.5f, 0.1f);
        }
        else
        {
            cardVisuals.FadeOverlay(0, 0.1f);
        }
    }

    public void Destroy(float delay = 0)
    {
        if (_isDestroyingSelf)
        {
            return;
        }

        _isDestroyingSelf = true;

        StartCoroutine(DestroyCoroutine(delay));
    }

    IEnumerator DestroyCoroutine(float delay = 0)
    {
        yield return new WaitForSeconds(delay);

        if (_doingDissolveAnimation)
        {
            yield return new WaitUntil(() => !_doingDissolveAnimation);
        }

        Destroy(gameObject);
    }
}
