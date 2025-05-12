using UnityEngine;
public class SafeArea : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Rect _safeArea;
    private Vector2 _minAnchor;
    private Vector2 _maxAnchor;

    private Vector2Int _oldScreenSize;

    private void Awake()
    {
        _rectTransform = transform as RectTransform;

        UpdateSafeArea();
    }

    private void Update()
    {
        if (_oldScreenSize.x == Screen.width && _oldScreenSize.y == Screen.height)
        {
            return;
        }

        UpdateSafeArea();
    }

    private void UpdateSafeArea()
    {
        _oldScreenSize.x = Screen.width;
        _oldScreenSize.y = Screen.height;

        _safeArea = Screen.safeArea;
        _minAnchor = _safeArea.position;
        _maxAnchor = _minAnchor + _safeArea.size;

        _minAnchor.x /= Screen.width;
        _minAnchor.y /= Screen.height;
        _maxAnchor.x /= Screen.width;
        _maxAnchor.y /= Screen.height;

        _rectTransform.anchorMin = _minAnchor;
        _rectTransform.anchorMax = _maxAnchor;
    }
}