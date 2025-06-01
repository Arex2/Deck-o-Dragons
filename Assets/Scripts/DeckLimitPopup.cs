using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DeckLimitPopup : MonoBehaviour
{
    [SerializeField] private float timeToDisapear;
    [SerializeField] private float disapearPosY;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Popup()
    {
        rectTransform.DOAnchorPosY(0, 0.6f);
        CancelInvoke(nameof(Popdown));
        Invoke(nameof(Popdown), timeToDisapear);
    }

    private void Popdown()
    {
        rectTransform.DOAnchorPosY(disapearPosY, 0.5f);
    }
}
