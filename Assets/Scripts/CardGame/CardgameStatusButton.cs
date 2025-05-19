using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class CardgameStatusButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public const string INTRO = "Battle Start";
    public const string PLAYER_TURN = "Your Turn";
    public const string ENEMY_TURN = "Enemy Turn";
    public const string DISCARD = "Discard";

    [SerializeField] private RectTransform rotationTransform;

    [Space]
    [SerializeField] private RectTransform warpTextsParent;
    [SerializeField] private WarpTextOnCircle warpTextTemplate;
    [SerializeField] private Transform seperatorTemplate;

    [Space]
    [SerializeField] private RectTransform radialCirclesParent;
    [SerializeField] private Image radialCircleTemplate;
    private float _radius;

    [Space]
    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color unselectedColor = Color.gray;

    [Space]
    [SerializeField] private int amountPerSide = 2;
    [SerializeField] private float anglePer = 145;

    [Header("Pressing")]
    [SerializeField] private float timeHeldUntilPress;
    [SerializeField] private float heldSize = 1.05f;
    [SerializeField] private float successPressSize = 1.12f;

    [Space]
    [SerializeField] private Image pressOverlay;
    [SerializeField] private float pressOverlayActiveAlpha;

    [Space]
    [SerializeField] private Image pressRadialFill;
    [SerializeField] private float pressRadialFillMin;
    [SerializeField] private float pressRadialFillMax;
    private float _pressRadialFillActiveAlpha;

    [Space]
    [SerializeField] private GameBehaviour gameBehaviour;

    private float _heldTimer;
    private bool _pressed;

    private int _length;
    private WarpTextOnCircle[] _allWarpTexts;
    private Image[] _allRadialCircles;

    private float _currentRotOffset;
    private float _oldRotOffset;

    public string PreviousStatus { get; private set; } = string.Empty;
    public string CurrentStatus { get; private set; } = string.Empty;
    public string UpcomingStatus { get; private set; } = string.Empty;

    private string _overrideCurrentText = string.Empty;

    private int _textOffset;
    private float _imageOffset;
    private Tween _currentRotationTween;
    private Tween _currentImageTween;

    private void Start()
    {
        _pressRadialFillActiveAlpha = pressRadialFill.color.a;
        pressRadialFill.fillAmount = pressRadialFillMin;

        _radius = warpTextTemplate.Radius;

        _length = amountPerSide * 2 + 1;
        _allWarpTexts = new WarpTextOnCircle[_length];
        _allRadialCircles = new Image[_length];
        int index = 0;

        for (int i = 0; i < 3; i++)
        {
            Transform seperator = Instantiate(seperatorTemplate, rotationTransform);

            seperator.rotation = Quaternion.Euler(0, 0, ((float)i * -anglePer) + (anglePer / 2));
            seperator.gameObject.SetActive(true);
        }

        WarpTextOnCircle Create(float angularOffset)
        {
            WarpTextOnCircle warpText = Instantiate(warpTextTemplate, warpTextsParent);

            warpText.Radius = _radius;
            warpText.AngularOffset = angularOffset;

            Image image = Instantiate(radialCircleTemplate, radialCirclesParent);

            _allWarpTexts[index] = warpText;
            _allRadialCircles[index] = image;
            index++;

            image.transform.localRotation = Quaternion.Euler(0, 0, -angularOffset + (anglePer / 2));
            image.fillAmount = anglePer / 360f;

            return warpText;
        }

        void CreateWarpTextOneSide(bool reverse)
        {
            for (int i = reverse ? (amountPerSide - 1) : 0; reverse ? i >= 0 : i < amountPerSide;  i += reverse ? -1 : 1)
            {
                WarpTextOnCircle warpText = Create(anglePer * (float)(i + 1) * (reverse ? -1f : 1f));
                warpText.name = "WarpText " + (reverse ? "Left" : "Right") + " (" + index + ")";
            }
        }

        CreateWarpTextOneSide(true);

        WarpTextOnCircle warpText = Create(0);
        warpText.name = "WarpText Middle (" + index + ")";

        CreateWarpTextOneSide(false);

        warpTextTemplate.gameObject.SetActive(false);
        radialCircleTemplate.gameObject.SetActive(false);
        seperatorTemplate.gameObject.SetActive(false);

        UpdateLooks();
    }

    private void Update()
    {
        if (_pressed)
        {
            _heldTimer += Time.deltaTime;

            //pressRadialFill.fillAmount = Mathf.Lerp(pressRadialFillMin, pressRadialFillMax, _heldTimer /  timeHeldUntilPress);

            if (_heldTimer > timeHeldUntilPress)
            {
                EndPress(true);
            }
        }

        if (_oldRotOffset == _currentRotOffset)
        {
            return;
        }

        UpdateLooks();
    }

    public void SetOverrideCurrentText(string text)
    {
        if (_overrideCurrentText == text)
        {
            return;
        }

        _overrideCurrentText = text;
        UpdateLooks();
    }


    public void ProceedStatus(string current, string upcoming)
    {
        if (CurrentStatus == current)
        {
            return;
        }

        PreviousStatus = CurrentStatus;
        CurrentStatus = current;
        UpcomingStatus = upcoming;

        if (_currentRotationTween != null)
        {
            _currentRotOffset = _currentRotOffset - anglePer;
            _imageOffset--;

            _currentRotationTween?.Kill();
        }

        if (_currentImageTween != null)
        {
            _currentImageTween?.Kill();
        }

        _textOffset = 1;

        _currentRotationTween = DOTween.To(() => _currentRotOffset, (value) => _currentRotOffset = value, anglePer, 1);
        _currentRotationTween.onComplete = () =>
        {
            _currentRotationTween = null;

            _currentRotOffset = 0;
            _textOffset = 0;
            _imageOffset = 0;

            UpdateLooks();
        };

        _currentImageTween = DOTween.To(() => _imageOffset, (value) => _imageOffset = value, 1, 0.5f);
        _currentImageTween.onComplete = () =>
        {
            _currentImageTween = null;
        };
    }

    public void UpdateLooks()
    {
        _oldRotOffset = _currentRotOffset;

        rotationTransform.rotation = Quaternion.Euler(0, 0, _currentRotOffset % anglePer);

        for (int i = 0; i < _length; i++)
        {
            WarpTextOnCircle warpText = _allWarpTexts[i];
            Image image = _allRadialCircles[i];

            float angle = warpText.AngularOffset - (_currentRotOffset % anglePer);
            float limit = anglePer + _radius;

            bool active = angle > -limit && angle < limit;

            warpText.gameObject.SetActive(active);
            image.gameObject.SetActive(active);

            if (!active)
            {
                continue;
            }

            int textIndex = (i + amountPerSide - _textOffset) % 3;

            warpText.Text = textIndex switch
            {
                0 => PreviousStatus,
                2 => UpcomingStatus,
                _ => string.IsNullOrEmpty(_overrideCurrentText) ? CurrentStatus : _overrideCurrentText,
            };

            float colorT = Mathf.Abs((((float)i + (float)amountPerSide - _imageOffset) % 3f) - 1);

            image.color = Color.Lerp(selectedColor, unselectedColor, colorT);

            warpText.UpdateText();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _heldTimer = 0;

        _pressed = true;

        pressOverlay.DOKill();
        pressOverlay.DOFade(pressOverlayActiveAlpha, 0.25f);

        pressRadialFill.DOKill(true);
        pressRadialFill.DOFade(_pressRadialFillActiveAlpha, 0.1f);
        pressRadialFill.DOFillAmount(pressRadialFillMax, timeHeldUntilPress).SetEase(Ease.Linear);

        transform.DOKill();
        transform.DOScale(heldSize, 0.75f).SetEase(Ease.OutExpo);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        EndPress(false);
    }

    private void EndPress(bool success)
    {
        if (!_pressed)
        {
            return;
        }

        _pressed = false;

        pressRadialFill.DOKill();
        transform.DOKill();

        if (success)
        {
            gameBehaviour.ButtonPress();

            pressRadialFill.DOFade(0, 0.25f).onComplete = () =>
            {
                pressRadialFill.fillAmount = pressRadialFillMin;
            };

            transform.localScale = Vector3.one * successPressSize;
            transform.DOScale(1, 0.75f).SetEase(Ease.OutBounce);
        }
        else
        {
            pressRadialFill.DOFillAmount(pressRadialFillMin, 0.1f);
            transform.DOScale(1, 0.1f);
        }

        pressOverlay.DOKill();
        pressOverlay.DOFade(0, 0.25f);
    }
}
