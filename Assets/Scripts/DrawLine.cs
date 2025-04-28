using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawLine : MonoBehaviour
{
    [SerializeField]
    LineRenderer lineRenderer;
    [SerializeField]
    Camera cam;
    [SerializeField]
    Button obj1;
    [SerializeField]
    Button obj2;

    List<Vector3> positions;
    private void Start()
    {
        positions = new List<Vector3>(); 
    }

    private void DrawLineBetweenPoints(Vector3 startPoint, Vector3 endPoint)
    {
        ++lineRenderer.positionCount;
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
    }

    public void AddButtonToLine(Button button)
    {
        //add to Z because otherwise cant be seen...
        Vector3 pos = new Vector3(Camera.main.ScreenToWorldPoint(button.transform.position).x, Camera.main.ScreenToWorldPoint(button.transform.position).y, 20);
        AddPosition(pos);
    }

    private void AddPosition(Vector3 pos)
    {
        ++lineRenderer.positionCount;
        positions.Add(pos);
        Draw();
    }

    private void Draw()
    {
        for (int i = 0; i < positions.Count; i++)
        {
            lineRenderer.SetPosition(i, positions[i]);
        }
    }
}
