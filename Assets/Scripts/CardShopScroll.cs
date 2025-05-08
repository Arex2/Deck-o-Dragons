using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class CardShopScroll : MonoBehaviour
{
    public List<RectTransform> cardPositions = new List<RectTransform>();
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    private float disctanceBetwenCards;
    private bool initialized = false;
    private bool scrolling;
    private int indexOfShortestDistance;
    private float cennterX;
    void Start()
    {
        cennterX = GetComponent<RectTransform>().position.x;
    }
    void Update()
    {
        if(!initialized) return;

        if(scrolling)
        {
            float shortestDistance = 1000; //An arbetrary number
            indexOfShortestDistance = -1;
            foreach (RectTransform cardPosition in cardPositions)
            {
                float cardDistance = Mathf.Abs(cennterX - cardPosition.position.x);
                if(cardDistance < shortestDistance)
                {
                    shortestDistance = cardDistance;
                    indexOfShortestDistance = cardPositions.IndexOf(cardPosition);
                }
            }
        }
        else
        {
            Snap();
        }
    }

    public void Initialize()
    {
        float paddingX = (cardPositions[0].sizeDelta.x * (cardPositions[0].localScale.x - 1f)) / 2;
        float paddingY = (cardPositions[0].sizeDelta.y * (cardPositions[0].localScale.y - 1f)) / 2;
        
        HorizontalLayoutGroup layoutGroup = content.GetComponent<HorizontalLayoutGroup>();
        layoutGroup.padding.top = (int) Mathf.Round(paddingY);
        layoutGroup.padding.bottom = (int) Mathf.Round(paddingY);
        layoutGroup.padding.left = (int) Mathf.Round(paddingX);
        layoutGroup.padding.right = (int) Mathf.Round(paddingX);

        layoutGroup.spacing = (int) Mathf.Round(paddingX);

        disctanceBetwenCards = cardPositions[0].sizeDelta.x * (cardPositions[0].localScale.x);

        initialized = true;
    }

    public void BeginDrag()
    {
        scrolling = true;
    }

    public void EndDrag()
    {
        scrolling = false;
    }

    private void Snap()
    {
        content.localPosition = Vector3.Lerp(content.localPosition, new Vector3(-1 * indexOfShortestDistance * disctanceBetwenCards, content.localPosition.y, content.localPosition.z), 4 * Time.deltaTime);
        print(-1 * indexOfShortestDistance * disctanceBetwenCards);
    }


}
