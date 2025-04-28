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
    private float moveMaxTimer = 12f;
    private float moveMinTimer = 5f;
    private float moveTimer;



    /*[SerializeField] private Transform transformToAffect;
    [SerializeField] private SquashStretchAxis axisToAffect = SquashStretchAxis.Y;
    [SerializeField, Range(0, 1f)] private float animationDuration = 0.25f;
    [SerializeField] private bool canBeOverwritten;

    //[Flags]
    public enum SquashStretchAxis
    {
        None = 0,
        X = 1,
        Y = 2,
        Z = 3
    }

    [SerializeField] private float initialScale = 1f;
    [SerializeField] private float maximumScale = 1.3f;
    [SerializeField] private bool resetToInitialScaleAfterAnimation = true;

    [SerializeField]
    private AnimationCurve squashAndStretchCurve = new AnimationCurve
    (
        new Keyframe(8f, 0f),
        new Keyframe(0.5f, 1f),
        new Keyframe(1f, 0f)
    );

    [SerializeField] private bool looping;
    [SerializeField] private float loopingDelay = 0.5f;

    private Coroutine squashAndStretchCoroutine;
    private WaitForSeconds loopingDelayWaitForSeconds;
    private Vector3 initialScaleVector;

    private bool affectX => (axisToAffect & SquashStretchAxis.X) != 0;
    private bool affectY => (axisToAffect & SquashStretchAxis.Y) != 0;
    private bool affectZ => (axisToAffect & SquashStretchAxis.Z) != 0;



    private void Awake()
    {
        if(transformToAffect == null)
        {
            transformToAffect = transform;

            initialScaleVector = transformToAffect.localScale;
            loopingDelayWaitForSeconds = new WaitForSeconds(loopingDelay);
        }
    }

    private void CheckForAndStartCoroutine()
    {
        if(axisToAffect == SquashStretchAxis.None)
        {
            Debug.Log("No axis to affect", gameObject);
            return;
        }

        if(squashAndStretchCoroutine != null)
        {
            StopCoroutine(squashAndStretchCoroutine);
            if (resetToInitialScaleAfterAnimation)
            {
                transform.localScale = initialScaleVector;
            }
        }

        squashAndStretchCoroutine = StartCoroutine(SquashAndStretchEffect());
    }*/


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

        //if (transform.position == currentTarget.transform.position)

        if(moveTimer <= 0)
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