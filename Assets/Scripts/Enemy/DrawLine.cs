using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawLine : MonoBehaviour
{
    [SerializeField]
    LineRenderer lineRenderer;

    List<Vector3> positions = new List<Vector3>();

    private float percentageComplete = 0;

    public void AddButtonToLine(Button button)
    {
        Debug.Log("triggered");

        //add to Z because otherwise cant be seen...
        Vector3 pos = new Vector3(Camera.main.ScreenToWorldPoint(button.transform.position).x, Camera.main.ScreenToWorldPoint(button.transform.position).y, 20);

        if(positions.Count < 1)
        {
            AddPosition(pos);
            return;
        }

        //lägga till offset baserat på riktning av linje
        float dir =  pos.x - positions[positions.Count - 1].x;
        dir = dir > 0 ? 1 : dir;
        dir = dir < 0 ? -1 : dir;


        //byta ut 3 mot Random.Range(2, 5) ?? 
        LerpFromLatestPoint(positions[positions.Count-1], positions[positions.Count - 1] + new Vector3(3 *dir, 0, 0), pos, percentageComplete);
    }

    private void LerpFromLatestPoint(Vector3 a, Vector3 b, Vector3 c, float percentageComplete)
    {
        while (percentageComplete < 1)
        {
            Vector3 newPos = Bezier(a, b, c, percentageComplete);
            ++lineRenderer.positionCount;
            positions.Add(newPos);
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, newPos);
            //AddPosition(newPos);
            percentageComplete += 0.05f;
        }

    }

    private void AddPosition(Vector3 pos)
    {
        ++lineRenderer.positionCount;
        positions.Add(pos);
        Draw();
    }

    //Old, now only used for pos 1
    private void Draw()
    {
        for (int i = 0; i < positions.Count; i++)
        {
            lineRenderer.SetPosition(i, positions[i]);
        }
    }


    //för att skapa kurva mellan två punkter:
    //parts från detta: https://discussions.unity.com/t/how-to-get-a-smooth-curved-line-between-two-points-like-those-present-between-the-nodes-of-bolt-visual-scripting/246143/2
    Vector2 Bezier(Vector2 a, Vector2 b, float t)
    {
        return Vector2.Lerp(a, b, t);
    }

    Vector2 Bezier(Vector2 a, Vector2 b, Vector2 c, float t)
    {
        return Vector2.Lerp(Bezier(a, b, t), Bezier(b, c, t), t);
    }

    /*
private void DrawLineBetweenPoints(Vector3 startPoint, Vector3 endPoint)
{
    ++lineRenderer.positionCount;
    lineRenderer.SetPosition(0, startPoint);
    lineRenderer.SetPosition(1, endPoint);
}
*/

}
