using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DragonBehavior : MonoBehaviour
{
    //private static DragonBehavior DrBeInstance;
    private static Vector3 leftOuterBounds = new Vector3(-2.5f, 4.5f, 0);
    private static Vector3 rightOuterBounds = new Vector3(2.5f, -4.5f, 0);
    private Vector3 currentTargetPosition;
    //private GameObject currentTarget;
    //private Rigidbody2D rb;
    private SpriteRenderer srend;
    private float moveSpeed = 1;

    /*private void Awake()
    {
        if (DrBeInstance == null)
        {
            DrBeInstance = this;
            leftOuterBounds = new Vector3(-2.5f, 4.5f, 0);
            rightOuterBounds = new Vector3(2.5f, -4.5f, 0);
        }
        else
        {
            Destroy(gameObject);
        }
    }*/

    // Start is called before the first frame update
    void Start()
    {
        //rb = GetComponent<Rigidbody2D>();
        srend = GetComponent<SpriteRenderer>();
        //currentTarget = new GameObject();
        //currentTarget.transform.position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Dragon: " + transform.position);
        //Debug.Log("Target: " + currentTarget.transform.position);

        //if (transform.position == currentTarget.transform.position)
        if (transform.position == currentTargetPosition)
        {
            CreateNewTarget();
        }

        //transform.position = Vector2.MoveTowards(transform.position, currentTarget.transform.position, moveSpeed * Time.deltaTime);
        transform.position = Vector2.MoveTowards(transform.position, currentTargetPosition, moveSpeed * Time.deltaTime);
    }

    /*private void CreateNewTarget()
    {
        float xPos = Random.Range(leftOuterBounds.x, rightOuterBounds.x);
        float yPos = Random.Range(rightOuterBounds.y, leftOuterBounds.y);

        currentTargetPosition = new Vector3(xPos, yPos, 0);
        currentTarget.transform.position = currentTargetPosition;
    }*/

    private void CreateNewTarget()
    {
        float xPos = Random.Range(leftOuterBounds.x, rightOuterBounds.x);
        float yPos = Random.Range(rightOuterBounds.y, leftOuterBounds.y);

        currentTargetPosition = new Vector3(xPos, yPos, 0);
        //currentTarget.transform.position = currentTargetPosition;
    }
}
