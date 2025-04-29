using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupWindowButton : MonoBehaviour
{
    public PopupWindow PopupWindow { get; set; }

    public Action OnClicked { get; private set; }

    [CacheComponent]
    [SerializeField] private TMP_Text text;
    [CacheComponent]
    [SerializeField] private Button button;

    private void Start()
    {
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        OnClicked?.Invoke();

        PopupWindow.Close();
    }

    public void Set(PopupWindow.Data.Button buttonData)
    {
        Set(buttonData.Text, buttonData.OnClicked);
    }

    public void Set(string text, Action onClicked)
    {
        this.text.text = text;
        OnClicked = onClicked;
    }
}
