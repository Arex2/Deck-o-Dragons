using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using TMPro;
using DG.Tweening;

/// <summary>
/// Use any of the Open() methods on this class to open up a popup window with a title, message and buttons the player can click. <para/>
/// Useful for "Are you sure?" prompts for example.
/// </summary>
// Script by Ruben
[SingletonMode(true)]
public class PopupWindow : Singleton<PopupWindow> 
{
    /* EXAMPLE OF HOW TO USE OPEN:
    Open("Are you sure?", "This will make you die permanently!\n\nThat's a long time.", 
        ("Yes", () => Debug.Log("You ded")),
        ("No", () => Debug.Log("Okay you live")),
        ("Huh", () => Debug.Log("What?"))
        );
    */

    public static bool IsOpen { get; private set; }

    private static readonly Data _templateData = new();
    
    private ObjectPool<PopupWindowButton> _pool;
    private readonly List<PopupWindowButton> _activeButtons = new();
    
    [SerializeField] private CanvasGroup overlayCanvasGroup;
    [SerializeField] private CanvasGroup mainPanelGroup;
    private RectTransform _mainPanelRect;

    [Space]
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text message;

    [Space]
    [SerializeField] private PopupWindowButton templateButton;
    [SerializeField] private Transform buttonParent;

    protected override void Awake()
    {
        base.Awake();

        _mainPanelRect = mainPanelGroup.transform as RectTransform;

        overlayCanvasGroup.alpha = 0;
        mainPanelGroup.alpha = 0;

        overlayCanvasGroup.blocksRaycasts = false;
        mainPanelGroup.blocksRaycasts = false;

        _mainPanelRect.localScale = Vector3.zero;

        templateButton.gameObject.SetActive(false);
        _pool = new(CreateButton, OnGetButton, OnReleaseButton);
    }

    private PopupWindowButton CreateButton()
    {
        PopupWindowButton button = Instantiate(templateButton);
        button.PopupWindow = this;

        return button;
    }

    private void OnGetButton(PopupWindowButton button)
    {
        button.gameObject.SetActive(true);
        button.transform.SetParent(buttonParent);
    }

    private void OnReleaseButton(PopupWindowButton button)
    {
        button.gameObject.SetActive(false);
        button.transform.SetParent(templateButton.transform.parent);
    }

    public static void Open(string title, string message, string buttonText, Action onPressed)
    {
        Open(title, message, new Data.Button(buttonText, onPressed));
    }

    public static void Open(string title, string message, string buttonLeftText, Action onPressedLeft, string buttonRightText, Action onPressedRight)
    {
        Open(title, message, new Data.Button(buttonLeftText, onPressedLeft), new Data.Button(buttonRightText, onPressedRight));
    }

    public static void Open(string title, string message, params Data.Button[] buttons)
    {
        Open(title, message, (IEnumerable<Data.Button>)buttons);
    }

    public static void Open(string title, string message, params (string, Action)[] buttons)
    {
        IEnumerable<Data.Button> Enumerable()
        {
            foreach (var button in buttons)
            {
                yield return new(button.Item1, button.Item2);
            }
        }

        Open(title, message, Enumerable());
    }

    public static void Open(string title, string message, Action<int> onClickOption, params string[] options)
    {
        IEnumerable<Data.Button> Enumerable()
        {
            int length = options.Length;

            for (int i = 0; i < length; i++)
            {
                yield return new(options[i], () => onClickOption.Invoke(i));
            }
        }

        Open(title, message, Enumerable());
    }

    public static void Open(string title, string message, IEnumerable<Data.Button> buttons)
    {
        _templateData.Title = title;
        _templateData.Message = message;
        _templateData.Buttons.Clear();

        foreach (Data.Button button in buttons)
        {
            _templateData.Buttons.Add(button);
        }

        Open(_templateData);
    }

    public static void Open(Data popupData)
    {
        Instance.LocalOpen(popupData);
    }

    private void LocalOpen(Data popupData)
    {
        if (IsOpen)
        {
            return;
        }

        IsOpen = true;

        title.text = popupData.Title;
        message.text = popupData.Message;

        foreach (PopupWindowButton button in _activeButtons)
        {
            _pool.Release(button);
        }

        _activeButtons.Clear();

        foreach (Data.Button buttonData in popupData.Buttons)
        {
            PopupWindowButton button = _pool.Get();
            button.transform.localScale = Vector3.one;

            _activeButtons.Add(button);

            button.Set(buttonData);
        }

        overlayCanvasGroup.DOKill();
        overlayCanvasGroup.DOFade(1, 0.5f);
        overlayCanvasGroup.blocksRaycasts = true;

        mainPanelGroup.blocksRaycasts = false;
        mainPanelGroup.alpha = 1;

        _mainPanelRect.DOKill();
        _mainPanelRect.DOScale(1, 0.5f).SetEase(Ease.OutBack).onComplete = () => mainPanelGroup.blocksRaycasts = true;
    }

    public void Close()
    {
        if (!IsOpen)
        {
            return;
        }

        IsOpen = false;
        
        overlayCanvasGroup.DOKill();
        overlayCanvasGroup.DOFade(0, 0.5f).onComplete = () => overlayCanvasGroup.blocksRaycasts = false;
        overlayCanvasGroup.blocksRaycasts = true;

        mainPanelGroup.blocksRaycasts = false;
        _mainPanelRect.DOKill();
        _mainPanelRect.DOScale(0, 0.35f).SetEase(Ease.InBack).onComplete = () => mainPanelGroup.alpha = 0;
    }

    public class Data
    {
        public string Title { get; set; }
        public string Message { get; set; }

        public List<Button> Buttons { get; private set; }

        public Data(string title, string message) : this()
        {
            Title = title;
            Message = message;
        }

        public Data()
        {
            Buttons = new();
        }

        public Data AddButton(string text, Action onClicked)
        {
            Buttons.Add(new(text, onClicked));

            return this;
        }

        public class Button
        {
            public string Text { get; set; }
            
            public Action OnClicked { get; set; }

            public Button(string text, Action onClicked)
            {
                Text = text;
                OnClicked = onClicked;
            }
        }
    }
}
