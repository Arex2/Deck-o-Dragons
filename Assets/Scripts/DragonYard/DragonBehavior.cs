using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DragonBehavior : MonoBehaviour
{
    private SpriteRenderer srend;

    [Header("Dragon Noises")]
    [SerializeField] private AudioClip[] dragonSqueals;

    [Header("Movement vectors")]
    //private static Vector3 leftOuterBounds = new Vector3(-1.5f, 4f, 0);
    //private static Vector3 rightOuterBounds = new Vector3(1.5f, -4f, 0);
    private Vector3 bottomLeft;
    private Vector3 topRight;
    private Vector3 currentTargetPosition;
    private Vector3 mousePosition;

    [Header("Movement related")]
    private float moveSpeed = 1f;
    private float moveMaxTimer = 10f;
    private float moveMinTimer = 5f;
    private float moveTimer;
    public float horizontalValue;
    private float zAxis = 0f;
    private bool canMove = true;
    public static bool canMakeNoise = true;

    void Start()
    {
        bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0.08f, 0.08f, 0));
        topRight = Camera.main.ViewportToWorldPoint(new Vector3(0.92f, 0.92f, 0));

        srend = GetComponent<SpriteRenderer>();
        moveTimer = Random.Range(moveMinTimer, moveMaxTimer);
    }

    void Update()
    {
        horizontalValue = transform.position.x;

        CheckIfOutOfBounds();

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
        if (canMove)
        {
            if (moveTimer > 0)
            {
                transform.position = Vector2.MoveTowards(transform.position, currentTargetPosition, moveSpeed * Time.deltaTime);
                moveTimer -= Time.deltaTime;
            }
        }
    }

    private void CheckIfOutOfBounds()
    {
        if (transform.position.x < bottomLeft.x)
        {
            transform.position = new Vector3(bottomLeft.x, transform.position.y, 0);
        }

        if (transform.position.x > topRight.x)
        {
            transform.position = new Vector3(topRight.x, transform.position.y, 0);
        }

        if (transform.position.y > topRight.y)
        {
            transform.position = new Vector3(transform.position.x, topRight.y, 0);
        }

        if (transform.position.y < bottomLeft.y)
        {
            transform.position = new Vector3(transform.position.x, bottomLeft.y, 0);
        }
    }

    public void OnMouseDown()
    {
        if (canMakeNoise)
        {
            int randomNoise = Random.Range(0, dragonSqueals.Length);
            AudioManager.Instance.PlayIncrVolumeSFX(dragonSqueals[randomNoise]);
        }
    }

    public void OnMouseHold(Vector2 mousePos)
    {
        if (canMakeNoise)
        {
            mousePosition = Camera.main.ScreenToWorldPoint(mousePos);
            mousePosition.z = zAxis;
            canMove = false;
            transform.position = mousePosition;
            //transform.position = mousePosition;
        }
    }

    public void OnMouseExit()
    {
        canMove = true;
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
        float xPos = Random.Range(bottomLeft.x, topRight.x);
        float yPos = Random.Range(bottomLeft.y, topRight.y);

        //float xPos = Random.Range(leftOuterBounds.x, rightOuterBounds.x);
        //float yPos = Random.Range(rightOuterBounds.y, leftOuterBounds.y);

        currentTargetPosition = new Vector3(xPos, yPos, 0);
        //currentTarget.transform.position = currentTargetPosition;
    }
}