using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class CardShopScroll : MonoBehaviour
{
    [SerializeField] private float scrollStrength = 4;
    [Space]
    public List<RectTransform> cardPositions = new List<RectTransform>();
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    private float disctanceBetwenCards;
    private bool initialized = false;
    private bool scrolling;
    public int indexOfShortestDistance {private set; get;}
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
        float paddingX = (cardPositions[0].sizeDelta.x * (cardPositions[0].localScale.x - 1f));     
        // HorizontalLayoutGroup layoutGroup = content.GetComponent<HorizontalLayoutGroup>();
        // layoutGroup.padding.top = (int) Mathf.Round(paddingY);
        // layoutGroup.padding.bottom = (int) Mathf.Round(paddingY);
        // layoutGroup.padding.left = (int) Mathf.Round(paddingX);
        // layoutGroup.padding.right = (int) Mathf.Round(paddingX);

        // layoutGroup.spacing = (int) Mathf.Round(paddingX);

        disctanceBetwenCards = paddingX + cardPositions[0].sizeDelta.x;//Mathf.Abs(cardPositions[0].position.x - cardPositions[1].position.x); //

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

    public void RemoveCurrentCard()
    {
        RectTransform cardToRemove = cardPositions[indexOfShortestDistance];
        cardPositions.Remove(cardToRemove);
        Destroy(cardToRemove.gameObject);
    }

    private void Snap()
    {
        float snapPosition = -1 * indexOfShortestDistance * disctanceBetwenCards;
        if(snapPosition > 0)
        {
            snapPosition = 0;
        }
        else if(snapPosition <= -1 * cardPositions.Count * disctanceBetwenCards)
        {
            snapPosition = -1 * (cardPositions.Count - 1) * disctanceBetwenCards;
        }
        content.localPosition = Vector3.Lerp(content.localPosition, new Vector3(snapPosition, content.localPosition.y, content.localPosition.z), scrollStrength * Time.deltaTime);
    }


}
