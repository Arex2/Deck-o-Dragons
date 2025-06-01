using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackYardButton : MonoBehaviour
{
    [CacheComponent]
    [SerializeField] private CanvasGroup canvasGroup;

    [Space]
    [SerializeField] private float checkTimer = 0.5f;
    private float _timer;

    private bool _currentEnabled;

    private void Start()
    {
        bool enabled = IsEnabled();
        _currentEnabled = enabled;

        canvasGroup.alpha = enabled ? 1 : 0;
        canvasGroup.blocksRaycasts = enabled;
    }

    private void Update()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
            return;
        }

        _timer = checkTimer;

        bool enabled = IsEnabled();

        if (_currentEnabled == enabled)
        {
            return;
        }

        _currentEnabled = enabled;

        canvasGroup.DOKill();
        canvasGroup.DOFade(enabled ? 1 : 0, 0.5f);
        canvasGroup.blocksRaycasts = enabled;
    }

    public bool IsEnabled() => DragonBookContents.GetDragonNamesInBook().Count > 0;
}
