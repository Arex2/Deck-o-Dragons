using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keyboard : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransf;

    [Range(0f, 1f)]
    [SerializeField] private float widthPercent;
    [Range(0f, 1f)]
    [SerializeField] private float heightPercent;
    [Range(0f, 5f)]
    [SerializeField] private float bottomOffSet;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return null;

        UpdateRectTransform();

        //rectTransf.sizeDelta = new Vector2(Screen.width, Screen.height / 2);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRectTransform();
    }

    private void UpdateRectTransform()
    {
        float width = widthPercent * Screen.width;
        float height = heightPercent * Screen.height;

        //Deciding the size of the keyboard box
        rectTransf.sizeDelta = new Vector2(width, height);

        //Decide the bottom offset
        Vector2 pos;
        pos.x = Screen.width/2;
        pos.y = bottomOffSet * Screen.height + height/2;

        rectTransf.position = pos;
    }
}