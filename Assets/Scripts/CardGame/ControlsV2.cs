using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ControlsV2 : MonoBehaviour
{

    public InputSystem controls;

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
    private float minSwipeSize = 1f;

    private bool firstContact = true;


    private int startSelectedIndex;
    private Vector2 startCurrentPos;

    private void OnEnable()
    {
        controls = new InputSystem();
        controls.CardGame.Disable();
        controls.CardMovement.Enable();

        controls.CardMovement.Position.performed += ctx => { OnContact(ctx.ReadValue<Vector2>()); };
        controls.CardMovement.Direction.performed += ctx => { dragDir = ctx.ReadValue<Vector2>(); };

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
                    Debug.LogWarning("Change in direction!");
                    hand.SetStartFloat();
                   // hand.SetStart();
                    SaveEdgeContactPoint();

                }
                dragDirection = Direction.Right;

            }
            else //LEFT
            {
                if (dragDirection != Direction.Left)
                {
                    Debug.LogWarning("Change in direction!");
                    hand.SetStartFloat();
                    //hand.SetStart();
                    SaveEdgeContactPoint();

                }
                dragDirection = Direction.Left;
            }
        }
        else
        {
            if (dragDir.y > 0) { dragDirection = Direction.Up; }
            else               { dragDirection = Direction.Down; }
        }
    }

    //method to save edge poisition
    private void SaveEdgeContactPoint()
    {
        latestEdgeContactPoint = currentContactPoint;
        latestEdgeContactTime = Time.time;
    }

    //method calculates how far to scroll after contact release
    private float CalculatePosToMoveTowards()
    {
        float change;
        change = latestEdgeContactPoint.x - currentContactPoint.x;
        //change = dragDir.magnitude
        /*
        if (currentContactPoint.x > startContactPoint.x)
            change = (0.5f - currentContactPoint.x) * 6;
        else change = (0.5f - currentContactPoint.x) * 6;
        */

        //length of swipe
        float length = latestEdgeContactPoint.x - currentContactPoint.x;

        //speed of swipe
        float speed = Time.time - latestEdgeContactTime;

        change = length * (dragDir.magnitude/100);// (speed/100); //desto kortare tid desto längre

       // Debug.Log("Speed: " + speed + "  length: " + length + "  dragDir: " + dragDir.magnitude);
        //add speed of swipe
        change *= 2;

        return change;
        //return position;
    }

    private void OnFirstContact(Vector2 t)
    {
        //Debug.LogWarning("OnFirstContact() at: " + Time.time);
        //save startContactPoint
        startContactPoint = currentContactPoint;
        latestEdgeContactPoint = startContactPoint;
        //Debug.Log("Start Pos: " + startContactPoint);
        //save startContactTime
        startContactTime = Time.time;

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

        if(firstContact)
        {
            OnFirstContact(currentContactPoint);
            firstContact = false;
        }

        DetectDirectionChange();


        //Debug.Log("DragDir magnitude: " + Mathf.Abs(dragDir.magnitude));

        //check if large enough touch/movements
        if (Mathf.Abs(dragDir.magnitude) < minSwipeSize)
            return;

        //check if direction is horizontal
        if (dragDirection.Equals(Direction.Up) || dragDirection.Equals(Direction.Down))
            return;


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
        change -= camera.ViewportToWorldPoint(new Vector3(0.5f,0,0)).x;

        change *= scrollStrength; 
        //change -= startContactPoint.x;
        //change -= star
        //change += startSelectedIndex;
        //Debug.Log("Change = " + change);
        hand.ShiftCards(change);

    }

    //when finger stops contact with screen
    //happens once
    private void OnRelease()
    {
        //Debug.Log("OnRelease() at: " + Time.time);
        //cardHand.movecards using CalculatePosToMoveTowards();
        //hand.SetStart();
        float change = CalculatePosToMoveTowards();

        //hand.ShiftCards(change);
        hand.SnapIntoPosition();

        firstContact = true;
    }

}
