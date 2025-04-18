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

    private float scrollStrength = 6;

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
        if (Mathf.Abs(currentContactPoint.x) > Mathf.Abs(dragDir.y))
        {
            if (dragDir.x > 0) //RIGHT
            {
                if (dragDirection != Direction.Right)
                {
                    Debug.LogWarning("Change in direction!");
                    SaveEdgeContactPoint();
                    hand.SetStart();
                }
                dragDirection = Direction.Right;

            }
            else //LEFT
            {
                if (dragDirection != Direction.Left)
                {
                    Debug.LogWarning("Change in direction!");
                    SaveEdgeContactPoint();
                    hand.SetStart();
                }
                dragDirection = Direction.Left;
            }
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
        //change = dragDir.magnitude
        if (currentContactPoint.x > startContactPoint.x)
            change = (0.5f - currentContactPoint.x) * 6;
        else change = (0.5f - currentContactPoint.x) * 6;


        //length of swipe
        float lenght = Mathf.Abs(latestEdgeContactPoint.x - currentContactPoint.x);

        //speed of swipe
        float speed = Time.time - latestEdgeContactTime;

        //Debug.Log("Speed: " + speed + "  length: " + lenght + "  dragDir: " + dragDir);
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
        Debug.Log("Start Pos: " + startContactPoint);
        //save startContactTime
        startContactTime = Time.time;

        startSelectedIndex = hand.SelectedIndex;
        startCurrentPos = hand.CurrentPos;
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

        //card hand move cards
        float change; //camera.ScreenToViewportPoint(new Vector2(Mathf.Abs(currentContactPoint.x), 0f)).x  - camera.ScreenToViewportPoint(new Vector2(Mathf.Abs(startContactPoint.x), 0f)).x  * 6;
        //add current position to this, and only update current position on onfirstContact()


        change = latestEdgeContactPoint.x - currentContactPoint.x;// Mathf.Abs(latestEdgeContactPoint.x - currentContactPoint.x);
        //if (dragDirection.Equals(Direction.Right))
            //change *= -1;
        /*

        if (currentContactPoint.x > startContactPoint.x)
            change = (0.5f - currentContactPoint.x);// * 6;
        else change = (0.5f - currentContactPoint.x);// * 6;
        */

        change *= scrollStrength; 
        //change -= startContactPoint.x;
        //change -= star
        //change += startSelectedIndex;
        //Debug.Log("Change = " + change);
        hand.ShiftCards(change, startCurrentPos);

    }

    //when finger stops contact with screen
    //happens once
    private void OnRelease()
    {
        //Debug.Log("OnRelease() at: " + Time.time);
        //cardHand.movecards using CalculatePosToMoveTowards();

        //float change = CalculatePosToMoveTowards();

        //hand.ShiftCards(change, startCurrentPos);
        hand.SnapIntoPosition();

        firstContact = true;
    }

}
