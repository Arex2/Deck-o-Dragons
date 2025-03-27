using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class Controlls : MonoBehaviour
{
    InputSystem controls;
    Vector2 dragDir;
    Vector2 swipe;
    float minSwipeSize = 10f;

    private void OnEnable()
    {
        controls = new InputSystem();
        controls.Enable();

        controls.test.Touch.canceled += SwipeAction;
        controls.test.ButtonPress.performed += ggg => { Debug.Log("Screen press"); };

        controls.test.Swipe.performed += ctx =>
        {
            GetSwipeDirection(ctx.ReadValue<Vector2>());
            //swipeDir = ctx.ReadValue<Vector2>();
            //ChooseAction();

        };
        //ANTINGEN OVAN eller NEDAN, båda kan användas för att känna direction of swipe
        controls.test.press.performed += ctx => 
        {
             //GetSwipeDirection(ctx.ReadValue<TouchState>());
        };// GetSwipeDirection(ctx.ReadValue<TouchState>()); };// Debug.Log("Current pos: " + ctx.ReadValue<TouchState>().position + "  start pos: " + ctx.ReadValue<TouchState>().startPosition); };




        //controls.test.press.canceled += SwipeAction;
        //controls.test.phonetest.performed += ctx => { Debug.Log(ctx.ReadValue<float>()); };
        //controls.test.PressingWithMouse.performed += ctx => { //ChooseAction();};
    }
    private void SwipeAction(InputAction.CallbackContext c)
    {
        //check if large enough touch
        if (Mathf.Abs(dragDir.magnitude) < minSwipeSize)
            return;
        Debug.Log("Swipe, direction: " + swipe);
        //NOLLSTÄLL
        dragDir = Vector2.zero;
    }

    private void GetSwipeDirection(TouchState touch)
    {
        //Vector3 AB = B - A.Destination - Origin.
        dragDir = touch.position - touch.startPosition;
        //dragDir.Normalize();
        //Debug.Log(direction);
        //if x axis has more input(?) than y
        if(Mathf.Abs(dragDir.x) > Mathf.Abs(dragDir.y))
        {
            if(dragDir.x > 0) //RIGHT
            {
                //Debug.Log("RIGHT");
                swipe = Vector2.right;
            }
            else //LEFT
            {
                //Debug.Log("LEFT");
                swipe = Vector2.left;
            }
        }
        else //y has more input
        {
            if(dragDir.y > 0) //UP
            {
                //Debug.Log("UP");
                swipe = Vector2.up;
            }
            else //DOWN
            {
                //Debug.Log("DOWN");
                swipe = Vector2.down;
            }
        }
    }
    private void GetSwipeDirection(Vector2 touch)
    {
        //Vector3 AB = B - A.Destination - Origin.
        dragDir = touch;
        //dragDir.Normalize();
        //Debug.Log(direction);
        //if x axis has more input(?) than y
        if (Mathf.Abs(dragDir.x) > Mathf.Abs(dragDir.y))
        {
            if (dragDir.x > 0) //RIGHT
            {
                //Debug.Log("RIGHT");
                swipe = Vector2.right;
            }
            else //LEFT
            {
                //Debug.Log("LEFT");
                swipe = Vector2.left;
            }
        }
        else //y has more input
        {
            if (dragDir.y > 0) //UP
            {
                //Debug.Log("UP");
                swipe = Vector2.up;
            }
            else //DOWN
            {
                //Debug.Log("DOWN");
                swipe = Vector2.down;
            }
        }
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ChooseAction()
    {
        //Debug.Log("controls value: " + swipeDir);
        
        if(dragDir.y > 0.6)
        {
            Debug.Log("UP");
        }
        else if (dragDir.y < -0.6)
        {
            Debug.Log("DOWN");
        }
        else
        {
            if (dragDir.x > 0)
            {
                Debug.Log("RIGHT");
                return;
            }
            else if (dragDir.x < 0)
            {
                Debug.Log("LEFT");
                return;
            }
        }
    }
}
