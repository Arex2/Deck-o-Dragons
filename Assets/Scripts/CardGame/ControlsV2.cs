using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private bool firstContact = true;

    private void OnEnable()
    {
        controls = new InputSystem();
        //controls.Enable();
        controls.CardGame.Disable();
        controls.CardMovement.Enable();
        //controls.CardMovement.Contact.started += ctx => {  OnFirstContact(ctx.ReadValue<Touch>()); };
        //controls.CardMovement.Position.started += ctx => { OnFirstContact(ctx.ReadValue<Vector2>()); };

        controls.CardMovement.Position.performed += ctx => { OnContact(ctx.ReadValue<Vector2>()); };
        controls.CardMovement.Direction.performed += ctx => { dragDir = ctx.ReadValue<Vector2>(); };
        //controls.CardMovement.Position.canceled += ctx => { OnRelease(); };

        controls.CardMovement.Contact.canceled += ctx => { OnRelease(); };
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    //method to detect when finger drag changes direction
    private void DetectDirectionChange()
    {
        //compare current pos with the pos before
        //to get a new direction
        //compare new direction with current direction 

        if (Mathf.Abs(currentContactPoint.x) > Mathf.Abs(dragDir.y))
        {
            if (dragDir.x > 0) //RIGHT
            {
                if (dragDirection != Direction.Right)
                {
                    Debug.LogWarning("Change in direction!");
                    //if change:
                    SaveEdgeContactPoint();
                }
                dragDirection = Direction.Right;

            }
            else //LEFT
            {
                if (dragDirection != Direction.Left)
                {
                    Debug.LogWarning("Change in direction!");
                    //if change:
                    SaveEdgeContactPoint();
                }
                dragDirection = Direction.Left;
            }
        }

    }

    //method to save edge poisition
    private void SaveEdgeContactPoint()
    {
        //save position (X is the important part)
        latestEdgeContactPoint = currentContactPoint;
        //save current time
        latestEdgeContactTime = Time.time;
    }

    //method calculates how far to scroll after contact release
    private void CalculatePosToMoveTowards()
    {
        //return position;
    }

    private void OnFirstContact(Vector2 t)
    {
        Debug.LogWarning("OnFirstContact() at: " + Time.time);
        //save startContactPoint
        startContactPoint = currentContactPoint;
        Debug.Log("Start Pos: " + startContactPoint);
        //save startContactTime
        startContactTime = Time.time;
    }

    //when finger is in contact with screen
    //happens continously
    private void OnContact(Vector2 pos)
    {
        //check so OnContact wont happen when no contact
        if (!controls.CardMovement.Contact.inProgress) return;

        currentContactPoint = pos;

        if(firstContact)
        {
            OnFirstContact(currentContactPoint);
            firstContact = false;
        }
        //controls.CardMovement.Contact.started += ctx => { OnFirstContact(ctx.ReadValue<Vector2>()); };
            


        Debug.Log("OnContact() at: " + Time.time);

        DetectDirectionChange();

        //card hand move cards
        float change; //camera.ScreenToViewportPoint(new Vector2(Mathf.Abs(currentContactPoint.x), 0f)).x  - camera.ScreenToViewportPoint(new Vector2(Mathf.Abs(startContactPoint.x), 0f)).x  * 6;
        //add current position to this, and only update current position on onfirstContact()
        if (currentContactPoint.x > startContactPoint.x)
            change = (0.5f - camera.ScreenToViewportPoint(new Vector2((currentContactPoint.x), 0f)).x) * 6;
        else change = (0.5f - camera.ScreenToViewportPoint(new Vector2((currentContactPoint.x), 0f)).x) * 6;
        change += camera.ScreenToViewportPoint(new Vector2((startContactPoint.x), 0f)).x;
        Debug.Log("Change = " + change);
        hand.ShiftCards(change);

    }

    //when finger stops contact with screen
    //happens once
    private void OnRelease()
    {
        Debug.Log("OnRelease() at: " + Time.time);
        //cardHand.movecards using CalculatePosToMoveTowards();

        firstContact = true;
    }

}
