using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class ControlsV2 : MonoBehaviour
{

    public InputSystem controls;

    [Header("Platform Settings")]
    [TextArea]
    public string Notes = "When using the mouse to test enable, when using a touchscreen to test, diable this. \n Touchscreens will be buggy unless this is disabled.";
    [SerializeField]
    public bool usingMouse;

    [Header("Connected Scripts")]
    [SerializeField]
    CardHand hand;

    [SerializeField]
    Camera camera;

    private Vector2 startContactPoint;
    private Vector2 latestEdgeContactPoint;
    private Vector2 currentContactPoint;
    private Vector2 dragDir;
    private Direction dragDirection; //current directiont finger is dragging

    private float startContactTime;
    private float latestEdgeContactTime;
    private float endContactTime;

    private float scrollStrength = 0.5f;
    private float minSwipeSize = 1f; //limit for movement being registered
    private float minReleasePower = 20f; //limit for movement being added to a release

    private bool firstContact = true;
    private bool swipingHorizontal;
    private bool swipeAxisRegistered;


    private int startSelectedIndex;
    private Vector2 startCurrentPos;

    //For movementAfterRelease
    private bool addReleaseMovement = false;
    private float additionalMoveVelocity;
    private float currentMoveVelocity;
    private float additionalMoveDuration = 0.5f;
    private float elapsedTime;

    [SerializeField]
    private AnimationCurve curve;

    private void Update()
    {
        if (addReleaseMovement)
        {
            elapsedTime += Time.deltaTime;
            float percentageComplete = elapsedTime / additionalMoveDuration;

            //if change == distToMove
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

    private void OnEnable()
    {
        controls = new InputSystem();
        controls.CardGame.Disable();
        controls.CardMovement.Enable();

        //disable these if using phone to test:
        //{
        if(usingMouse)
        {
            controls.CardMovement.Position1.performed += ctx => { OnContact(ctx.ReadValue<Vector2>()); };
            controls.CardMovement.Direction1.performed += ctx => { dragDir = ctx.ReadValue<Vector2>(); };
        }
        //controls.CardMovement.Position1.performed += ctx => { OnContact(ctx.ReadValue<Vector2>()); };
        //controls.CardMovement.Direction1.performed += ctx => { dragDir = ctx.ReadValue<Vector2>(); };
        //}

        controls.CardMovement.Position.performed += ctx => { 
            //if (!controls.CardMovement.Contact.inProgress) return; 
            OnContact(ctx.ReadValue<Vector2>()); };
        controls.CardMovement.Direction.performed += ctx => { 
            //if (!controls.CardMovement.Contact.inProgress) return; 
            dragDir = ctx.ReadValue<Vector2>(); };

        controls.CardMovement.Contact.canceled += ctx => { OnRelease(); };
    }

    private void OnDisable()
    {
        controls.Disable();
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
                    //hand.SetStartFloat();
                   // hand.SetStart();
                    SaveEdgeContactPoint();

                }
                dragDirection = Direction.Right;

            }
            else //LEFT
            {
                if (dragDirection != Direction.Left)
                {
                    //Debug.LogWarning("Change in direction!");
                    //hand.SetStartFloat();
                    //hand.SetStart();
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
            if (distX > distY)
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
        //Debug.Log("Start Pos: " + startContactPoint);
        //save startContactTime
        startContactTime = Time.time;

        //swipingHorizontal = true;
        //Debug.LogWarning("OnFirstContact() at: " + Time.time);
        //save startContactPoint


        startSelectedIndex = hand.SelectedIndex;
        startCurrentPos = hand.CurrentPos;
        hand.SetStartFloat();
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
        if (swipingHorizontal)//dragDirection.Equals(Direction.Up) || dragDirection.Equals(Direction.Down))
        {
            //card hand move cards
            float change; //camera.ScreenToViewportPoint(new Vector2(Mathf.Abs(currentContactPoint.x), 0f)).x  - camera.ScreenToViewportPoint(new Vector2(Mathf.Abs(startContactPoint.x), 0f)).x  * 6;
                          //add current position to this, and only update current position on onfirstContact()

            //translating viewport to game world distance
            //change = camera.ViewportToWorldPoint(latestEdgeContactPoint).x - camera.ViewportToWorldPoint(currentContactPoint).x;// Mathf.Abs(latestEdgeContactPoint.x - currentContactPoint.x);
            //if (dragDirection.Equals(Direction.Right))
            //change *= -1;

            change = camera.ViewportToWorldPoint(startContactPoint).x - camera.ViewportToWorldPoint(currentContactPoint).x;
            /*

            if (currentContactPoint.x > startContactPoint.x)
                change = (0.5f - currentContactPoint.x);// * 6;
            else change = (0.5f - currentContactPoint.x);// * 6;
            */
            change -= camera.ViewportToWorldPoint(new Vector3(0.5f, 0, 0)).x;

            change *= scrollStrength;
            //change -= startContactPoint.x;
            //change -= star
            //change += startSelectedIndex;
            //Debug.Log("Change = " + change);
            hand.ShiftCards(change);
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
                if(dragDirection == Direction.Down)
                    Debug.Log("Do something else (swipe down)");
                else Debug.Log("PlayCard");
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


        //hand.ContinueMoving(releaseVelocity);//.onCompleted = hand.SnapIntoPosition();
        //hand.SnapIntoPosition();
        //StartCoroutine(hand.ContinueMoving(releaseVelocity));
        firstContact = true;
        swipeAxisRegistered = false;
        //Debug.Log("swipe axis now set to = false");
    }

    //method calculates how far to scroll after contact release
    private float CalculateSpeedToMoveWith()
    {
        //length of swipe
        float distance =  currentContactPoint.x - latestEdgeContactPoint.x;

        //speed of swipe
        float time = Time.time - latestEdgeContactTime;

        float velocity = distance / time;  //(dragDir.magnitude/100);// (speed/100); //desto kortare tid desto längre

        // Debug.Log("Speed: " + speed + "  length: " + length + "  dragDir: " + dragDir.magnitude);

        // add modifier (?)
         velocity *= 0.3f;

        //velocity = distance;
        float moveAdditional = distance / time;
        moveAdditional *= 0.5f;
        velocity = camera.ViewportToWorldPoint(startContactPoint).x - (camera.ViewportToWorldPoint(currentContactPoint).x + moveAdditional);

        return velocity;
    }

    private void contMove(float dist)
    {
        bool run = true;
        float change = 0;
        while (run)
        {
            change = Mathf.Lerp(0, dist, 0.5f);
            hand.ShiftCards(change);
            if(change > dist) 
            {
                hand.SnapIntoPosition();
                run = false;
            }
        }
        /*
        float change = dist/6;
        int j = 0;
        for (int i = 0; i < dist; i++)
        {
            change += dist / 6;
            hand.ShiftCards(change);
            j= i;
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitUntil(() => j >= 10);
        hand.SnapIntoPosition();
        */
    }


}
