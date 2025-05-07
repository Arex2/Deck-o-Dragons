using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;
using UnityEngine.UI;

public class CardShopScroll : MonoBehaviour
{
    public List<RectTransform> cardPositions = new List<RectTransform>();
    [SerializeField] private ScrollRect scrollRect;
    private bool scrolling;
    private float cennterX;
    void Start()
    {
        cennterX = GetComponent<RectTransform>().position.x;
    }
    void Update()
    {
        Snap();
    }

    private void Snap()
    {
        float shortestDistance = 1000; //An arbetrary number
        foreach (RectTransform cardPosition in cardPositions)
        {
            float cardDistance = Mathf.Abs(cennterX - cardPosition.position.x);
            if(cardDistance < shortestDistance)
            {
                shortestDistance = cardDistance;
            }
        }
        print(shortestDistance);
    }
}
