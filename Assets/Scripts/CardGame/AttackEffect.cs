using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.XR;

public class AttackEffect : MonoBehaviour
{
    [SerializeField]
    Vector2 a, b, c, d;
    [SerializeField]
    GameObject enemy;
    Vector2 enemyPos;
    public Vector2 EnemyPos { set { enemyPos = value; d = enemyPos; } }


    float desiredDuration = 2f;
    float elapsedTime;

    //[SerializeField]
    //Vector3[] path;
    [SerializeField]
    ParticleSystem explosion;

    private bool once;

    // Start is called before the first frame update
    void Start()
    {
        a = gameObject.transform.position;
        b = new Vector2(Random.Range(-5, 5), Random.Range(-2, 2));
        c = new Vector2(Random.Range(-5,5), Random.Range(2,5));
        //d = enemy.transform.position;
        d = GameObject.Find("EnemyBoss").transform.position;
        //if(enemyPos != null)
        //d = enemyPos;

        //transform.DOPath(path, desiredDuration, PathType.CatmullRom);
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        float percentageComplete = elapsedTime / desiredDuration;

        //transform.position = Vector2.Lerp(a, b, percentageComplete);


        transform.position = Bezier(a, b, c, d, elapsedTime);

        //transform.DOJump(Vector3 endValue, float jumpPower, int numJumps, float duration, bool snapping)
        //transform.DOJump(b, 2, 5, desiredDuration);
        //, PathType.CatmullRom);
        /*
        if (new Vector2(transform.position.x, transform.position.y) == b)
        {
            //DestroySelf();
        }
        if(elapsedTime >= 1 && !once)//desiredDuration)
        {
            DestroySelf();
            once = true;
        }
        */
    }
    /*
    private void GeneratePath(Vector3 startPos, Vector3 endPos)
    {
        path = new Vector3[6];
        for(int i = 0; i < path.Length; i++)
        {
            if(i == 0)
                path[i] = startPos;
            else
            path[i] = path[i-1] + new Vector3(0, 0, 0);
        }
    }
    */
    Vector2 Bezier(Vector2 a, Vector2 b, float t)
    {
        return Vector2.Lerp(a, b, t);
    }

    Vector2 Bezier(Vector2 a, Vector2 b, Vector2 c, float t)
    {
        return Vector2.Lerp(Bezier(a, b, t), Bezier(b, c, t), t);
    }

    Vector2 Bezier(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t)
    {
        return Vector2.Lerp(Bezier(a, b, c, t), Bezier(b, c, d, t), t);
    }

    private void MoveTowards(Vector3 endPos)
    {

    }

    public void DestroySelf()
    {
        //instantiate explosion particle effect
        ParticleSystem g = Instantiate(explosion);
        g.transform.position = d;
        //delete self
        Destroy(gameObject,0.1f);
    }
}
