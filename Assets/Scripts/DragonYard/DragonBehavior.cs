using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DragonBehavior : MonoBehaviour
{
    //private static DragonBehavior DrBeInstance;
    private static Vector3 leftOuterBounds = new Vector3(-1.5f, 4f, 0);
    private static Vector3 rightOuterBounds = new Vector3(1.5f, -4f, 0);
    private Vector3 currentTargetPosition;
    //private GameObject currentTarget;
    //private Rigidbody2D rb;
    private SpriteRenderer srend;
    private float moveSpeed = 1;
    private float moveMaxTimer = 12f;
    private float moveMinTimer = 5f;
    private float moveTimer;
    public float horizontalValue;

    void Start()
    {
        //rb = GetComponent<Rigidbody2D>();
        srend = GetComponent<SpriteRenderer>();
        moveTimer = Random.Range(moveMinTimer, moveMaxTimer);


        //CheckForAndStartCoroutine();


        //currentTarget = new GameObject();
        //currentTarget.transform.position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Dragon: " + transform.position);
        //Debug.Log("Target: " + currentTarget.transform.position);

        horizontalValue = transform.position.x;

        if (moveTimer <= 0)
        {
            float waitTimer = Random.Range(2f, 5f);
            Invoke("CreateNewTimer", waitTimer);
        }

        if (transform.position == currentTargetPosition)
        {
            CreateNewTarget();
        }

        //transform.position = Vector2.MoveTowards(transform.position, currentTarget.transform.position, moveSpeed * Time.deltaTime);
        if(moveTimer > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, currentTargetPosition, moveSpeed * Time.deltaTime);
            moveTimer -= Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        if (transform.position.x < horizontalValue)
        {
            FlipSprite(false);
        }
        if (transform.position.x > horizontalValue)
        {
            FlipSprite(true);
        }
    }

    private void FlipSprite(bool direction)
    {
        srend.flipX = direction;
    }

    private void CreateNewTimer()
    {
        moveTimer = Random.Range(moveMinTimer, moveMaxTimer);
    }

    private void CreateNewTarget()
    {
        float xPos = Random.Range(leftOuterBounds.x, rightOuterBounds.x);
        float yPos = Random.Range(rightOuterBounds.y, leftOuterBounds.y);

        currentTargetPosition = new Vector3(xPos, yPos, 0);
        //currentTarget.transform.position = currentTargetPosition;
    }
}