using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;


enum Direction
{
    Right, Left, Up, Down
}

public class Controlls : MonoBehaviour
{
    InputSystem controls;
    Vector2 dragDir;
    //Vector2 swipe;
    float minSwipeSize = 10f;
    Direction swipeDirection;

    [SerializeField]
    CardHand hand;

    private void OnEnable()
    {
        Input.gyro.enabled = true;

        controls = new InputSystem();
        controls.Enable();

        controls.test.Touch.canceled += SwipeAction;
        controls.test.KeyboardAny.canceled += SwipeActionKeyboard;
        controls.test.Tap.performed += ctx => { Debug.Log("Screen tap"); };

        controls.test.Swipe.performed += ctx =>
        {
            GetSwipeDirection(ctx.ReadValue<Vector2>());
            //Debug.Log("aaaaaaaaaaaaaaaaaaaaaaaaaaaa" + ctx.ReadValue<Vector2>());
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
    private void SwipeActionKeyboard(InputAction.CallbackContext c)
    {
        Debug.Log("Swipe, direction: " + swipeDirection);
        SelectAction();
        //NOLLSTÄLL
        dragDir = Vector2.zero;
    }
    private void SwipeAction(InputAction.CallbackContext c)
    {
        //check if large enough touch
        if (Mathf.Abs(dragDir.magnitude) < minSwipeSize)
            return;
        Debug.Log("Swipe, direction: " + swipeDirection);
        SelectAction();
        //NOLLSTÄLL
        dragDir = Vector2.zero;
    }
    private void SelectAction()
    {
        switch(swipeDirection)
        {
            case Direction.Up:
                //play card
                Debug.Log("Play card");
                hand.PlayCard();
                break;
            case Direction.Down:
                //open card deck
                Debug.Log("Open card deck [PH]");
                break;
            case Direction.Right:
                //either switch to garden scene or scroll cards
                Debug.Log("either switch to garden scene or scroll cards [PH] -->");
                hand.ScrollRight();
                break;
            case Direction.Left:
                //scroll cards
                Debug.Log("<-- scroll cards [PH]");
                hand.ScrollLeft();
                break;
        }
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
            if (dragDir.x > 0) //RIGHT
            {
                //Debug.Log("RIGHT");
                //swipe = Vector2.right;
                swipeDirection = Direction.Right;
            }
            else //LEFT
            {
                //Debug.Log("LEFT");
                //swipe = Vector2.left;
                swipeDirection = Direction.Left;
            }
        }
        else //y has more input
        {
            if (dragDir.y > 0) //UP
            {
                //Debug.Log("UP");
                //swipe = Vector2.up;
                swipeDirection = Direction.Up;
            }
            else //DOWN
            {
                //Debug.Log("DOWN");
                //swipe = Vector2.down;
                swipeDirection = Direction.Down;
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
                //swipe = Vector2.right;
                swipeDirection = Direction.Right;
            }
            else //LEFT
            {
                //Debug.Log("LEFT");
                //swipe = Vector2.left;
                swipeDirection = Direction.Left;
            }
        }
        else //y has more input
        {
            if (dragDir.y > 0) //UP
            {
                //Debug.Log("UP");
                //swipe = Vector2.up;
                swipeDirection = Direction.Up;
            }
            else //DOWN
            {
                //Debug.Log("DOWN");
                //swipe = Vector2.down;
                swipeDirection = Direction.Down;
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
        //Debug.Log(Input.acceleration);// gyro.userAcceleration);
        if(UnityEngine.InputSystem.Gyroscope.current != null)
        {
            //Debug.Log(UnityEngine.InputSystem.Gyroscope.current.angularVelocity.ReadValue());
        }
    }

    private void CheckForShake()
    {
        Vector3 acceleration = Input.acceleration; //mobil rörelse
    }

}
