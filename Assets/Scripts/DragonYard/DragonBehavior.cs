using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DragonBehavior : MonoBehaviour
{
    private SpriteRenderer srend;

    [Header("Dragon Noises")]
    [SerializeField] private AudioClip[] dragonSqueals;

    [Header ("Movement vectors")]
    private static Vector3 leftOuterBounds = new Vector3(-1.5f, 4f, 0);
    private static Vector3 rightOuterBounds = new Vector3(1.5f, -4f, 0);
    private Vector3 currentTargetPosition;
    private Vector3 mousePosition;

    [Header ("Movement related")]
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
        srend = GetComponent<SpriteRenderer>();
        moveTimer = Random.Range(moveMinTimer, moveMaxTimer);
    }

    void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = zAxis;
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
        if(canMove)
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
        if (transform.position.x < leftOuterBounds.x)
        {
            transform.position = new Vector3(leftOuterBounds.x, transform.position.y, 0);
        }

        if (transform.position.x > rightOuterBounds.x)
        {
            transform.position = new Vector3(rightOuterBounds.x, transform.position.y, 0);
        }

        if (transform.position.y > leftOuterBounds.y)
        {
            transform.position = new Vector3(transform.position.x, leftOuterBounds.y, 0);
        }

        if (transform.position.y < rightOuterBounds.y)
        {
            transform.position = new Vector3(transform.position.x, rightOuterBounds.y, 0);
        }
    }

    private void OnMouseDown()
    {
        if (canMakeNoise)
        {
            int randomNoise = Random.Range(0, dragonSqueals.Length);
            AudioManager.Instance.PlayIncrVolumeSFX(dragonSqueals[randomNoise]);
        }
    }

    private void OnMouseDrag()
    {
        if(canMakeNoise)
        {
            canMove = false;
            transform.position = mousePosition;
        }
    }

    private void OnMouseExit()
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
        float xPos = Random.Range(leftOuterBounds.x, rightOuterBounds.x);
        float yPos = Random.Range(rightOuterBounds.y, leftOuterBounds.y);

        currentTargetPosition = new Vector3(xPos, yPos, 0);
        //currentTarget.transform.position = currentTargetPosition;
    }
}