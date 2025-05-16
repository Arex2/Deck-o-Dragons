//using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ControlsV2 : MonoBehaviour
{

    public InputSystem controls;

#if UNITY_EDITOR
    [Header("Platform Settings")]
    [TextArea]
    public string Notes = "When using the mouse to test enable, when using a touchscreen to test, disable this. \n Touchscreens will be buggy unless this is disabled.";
#endif
    [SerializeField]
    public bool usingMouse;

    [Header("Connected Scripts")]
    [SerializeField]
    CardHand hand;

    [SerializeField]
#if UNITY_EDITOR // This gets rid of an annoying warning in console
    new
#endif
    Camera camera;


    [SerializeField]
    SceneSwitcher sceneSwitcher;

    private Vector2 startContactPoint;
    private Vector2 latestEdgeContactPoint;
    private Vector2 currentContactPoint;
    private Vector2 dragDir;
    private Direction dragDirection; //current directiont finger is dragging

    //private float startContactTime;
    private float latestEdgeContactTime;
    //private float endContactTime;

    private float scrollStrength = 0.5f;
    private float minSwipeSize = 1f; //limit for movement being registered
    private float minReleasePower = 20f; //limit for movement being added to a release

#pragma warning disable CS0414
    float leftEdgeArea = 0.15f;
    float rightEdgeArea = 0.85f;
#pragma warning restore CS0414

    private bool firstContact = true;
    private bool swipingHorizontal;
    private bool swipeAxisRegistered;

    //For movementAfterRelease:
    private bool addReleaseMovement = false;
    private float additionalMoveVelocity;
    private float currentMoveVelocity;
    private float additionalMoveDuration = 0.5f;
    private float elapsedTime;

    [SerializeField]
    private AnimationCurve curve;

    private void OnEnable()
    {
        controls = new InputSystem();
        controls.CardMovement.Enable();

        if (usingMouse)
        {
            controls.CardMovement.Position1.performed += ctx => { OnContact(ctx.ReadValue<Vector2>()); };
            controls.CardMovement.Direction1.performed += ctx => { dragDir = ctx.ReadValue<Vector2>(); };
        }

        controls.CardMovement.Position.performed += ctx => { OnContact(ctx.ReadValue<Vector2>()); };
        controls.CardMovement.Direction.performed += ctx => { dragDir = ctx.ReadValue<Vector2>(); };

        controls.CardMovement.Contact.canceled += ctx => { OnRelease(); };
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        if (addReleaseMovement)
        {
            elapsedTime += Time.deltaTime;
            float percentageComplete = elapsedTime / additionalMoveDuration;

            if (Mathf.Abs(currentMoveVelocity) >= Mathf.Abs(additionalMoveVelocity) - 0.5f)
            {
                //Debug.Log("Reset stuff. current V :  " + currentMoveVelocity + " additional V:  " + additionalMoveVelocity);
                hand.SnapIntoPosition();
                //reset stuff
                ResetMovementCont();
            }
            else
            {
                //lerp change
                currentMoveVelocity = Mathf.Lerp(0, additionalMoveVelocity, curve.Evaluate(percentageComplete));
                //shiftcards
                hand.ShiftCards(currentMoveVelocity);
            }
        }
    }

    private void ResetMovementCont()
    {
        addReleaseMovement = false;
        currentMoveVelocity = 0;
        additionalMoveVelocity = 0;
        elapsedTime = 0;
    }



    //method to detect when finger drag changes direction
    private void DetectDirectionChange()
    {
        if (Mathf.Abs(dragDir.x) > Mathf.Abs(dragDir.y))
        {
            if (dragDir.x > 0) //RIGHT
            {
                if (dragDirection != Direction.Right)
                {
                    //Debug.LogWarning("Change in direction!");
                    SaveEdgeContactPoint();

                }
                dragDirection = Direction.Right;

            }
            else //LEFT
            {
                if (dragDirection != Direction.Left)
                {
                    //Debug.LogWarning("Change in direction!");
                    SaveEdgeContactPoint();

                }
                dragDirection = Direction.Left;
            }
        }
        else
        {
            if (dragDir.y > 0) 
            {
                //Debug.LogWarning("Up");
                dragDirection = Direction.Up; 
            }
            else               
            {
                //Debug.LogWarning("Down");
                dragDirection = Direction.Down; 
            }
        }
    }

    
    private void RegisterInitialMoveAxis()
    {
        //Debug.Log("RegisterInitialMoveAxis");
        Vector2 dist = startContactPoint - currentContactPoint;
        if(Mathf.Abs(dist.magnitude) > 0.1f)
        {
            //horizontal or vertical
            float distX = Mathf.Abs(startContactPoint.x - currentContactPoint.x); 
            float distY = Mathf.Abs(startContactPoint.y - currentContactPoint.y);
            if (distX > distY) //could compare dist.x and dist.y instead ??
            {
                //horizontal
                swipingHorizontal = true;
                Debug.Log("Swiping horizontal registered");
            }
            else
            {
                //vertical
                swipingHorizontal= false;
                Debug.Log("Swiping vertical registered");
            }
            swipeAxisRegistered = true;
        }
    }

    //method to save edge poisition
    private void SaveEdgeContactPoint()
    {
        latestEdgeContactPoint = currentContactPoint;
        latestEdgeContactTime = Time.time;
    }

    private void OnFirstContact(Vector2 t)
    {
        ResetMovementCont();

        startContactPoint = currentContactPoint;
        latestEdgeContactPoint = startContactPoint;
        latestEdgeContactTime = Time.time;
        //Debug.Log("Start Pos: " + startContactPoint);
        //save startContactTime
        //startContactTime = Time.time;

        //startSelectedIndex = hand.SelectedIndex;
        //startCurrentPos = hand.CurrentPos;
        hand.SetStart();
    }

    //when finger is in contact with screen
    //happens continously
    private void OnContact(Vector2 pos)
    {
        //check so OnContact wont happen when no contact
        if (!controls.CardMovement.Contact.inProgress) return;

        currentContactPoint = camera.ScreenToViewportPoint(pos);
        DetectDirectionChange();

        if (firstContact)
        {
            OnFirstContact(currentContactPoint);
            firstContact = false;
        }

        if(!swipeAxisRegistered)
        {
            RegisterInitialMoveAxis();
            return;
        }
        //Debug.Log("DragDir magnitude: " + Mathf.Abs(dragDir.magnitude));

        //check if large enough touch/movements
        if (Mathf.Abs(dragDir.magnitude) < minSwipeSize)
            return;

        //check if direction is horizontal
        if (swipingHorizontal)
        {
             MoveCards();
        }
    }

    //when finger stops contact with screen
    //happens once
    private void OnRelease()
    {
        //first check if not moving cards
        if(!swipingHorizontal && swipeAxisRegistered)
        {
            if(dragDir.magnitude > minSwipeSize)
            {
                //register if play card or other
                if (dragDirection == Direction.Down)
                    Debug.Log("Do something else (swipe down)");
                else
                {
                    Debug.Log("PlayCard");
                    hand.UseCurrentCard();
                }
            }
        }
        else
        {

            //cards stay still
            if (dragDir.magnitude < minReleasePower)
            {
                hand.SnapIntoPosition();
                firstContact = true;
                swipeAxisRegistered = false;
                return;
            }
            //cards get added movement
            additionalMoveVelocity = CalculateSpeedToMoveWith();
            addReleaseMovement = true;

            #region old sceneswitch
            /*
            //check start position for card drag or scene switch
            if (startContactPoint.x < leftEdgeArea)
            {
                if(dragDir.magnitude > minSwipeSize)
                {
                    Debug.Log("Switch scene RIGHT");
                    sceneSwitcher.SwitchToEgg();
                }
            }
            else if (startContactPoint.x > rightEdgeArea)
            {
                if (dragDir.magnitude > minSwipeSize)
                {
                    Debug.Log("Switch scene LEFT");
                    //sceneSwitcher.SwitchScene(+1);
                    // sceneSwitcher.SwitchToCardGame();
                    //sceneSwitcher.SwitchToGarden();
                }
            }
            else
            {
                //cards stay still
                if (dragDir.magnitude < minReleasePower)
                {
                    hand.SnapIntoPosition();
                    firstContact = true;
                    swipeAxisRegistered = false;
                    return;
                }
                //cards get added movement
                additionalMoveVelocity = CalculateSpeedToMoveWith();
                addReleaseMovement = true;
            }
            */
            #endregion

        }

        firstContact = true;
        swipeAxisRegistered = false;
        //Debug.Log("swipe axis now set to = false");
    }

    private void MoveCards()
    {
        //card hand move cards
        float change;
        change = camera.ViewportToWorldPoint(startContactPoint).x - camera.ViewportToWorldPoint(currentContactPoint).x;
        change -= camera.ViewportToWorldPoint(new Vector3(0.5f, 0, 0)).x;
        change *= scrollStrength;
        //Debug.Log("Change = " + change);
        hand.ShiftCards(change);
    }

    //method calculates how far to scroll after contact release
    private float CalculateSpeedToMoveWith()
    {
        //length of swipe
        float distance = latestEdgeContactPoint.x - currentContactPoint.x;//currentContactPoint.x - latestEdgeContactPoint.x;

        //speed of swipe
        float time = Time.time - latestEdgeContactTime;

        float velocity = distance / time;  //(dragDir.magnitude/100);// (speed/100); //desto kortare tid desto l�ngre

        //float moveAdditional = distance / time;
        //moveAdditional *= 0.5f;
        //float velocity = camera.ViewportToWorldPoint(startContactPoint).x - (camera.ViewportToWorldPoint(currentContactPoint).x + moveAdditional);

        //return velocity;
        float change = startContactPoint.x - currentContactPoint.x + velocity;// camera.ViewportToWorldPoint(startContactPoint).x - (camera.ViewportToWorldPoint(currentContactPoint).x - velocity);
        change = Mathf.Round(change);
        //Debug.Log("velocity: " + velocity + "  change:  " + change + "  .");
        return change;
    }
}
