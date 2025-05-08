using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;
using UnityEngine.UI;

public class CardShopScroll : MonoBehaviour
{
    public List<RectTransform> cardPositions = new List<RectTransform>();
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
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
        if(!scrolling)
        {
            print("yup");
            Snap();
        }
    }

    public void Initialize()
    {
        
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
        content.position = new Vector3(content.position.x, content.position.y, content.position.z);
    }


}
