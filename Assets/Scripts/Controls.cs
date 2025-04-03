using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;


enum Direction
{
    Right, Left, Up, Down
}

public class Controls : MonoBehaviour
{
    InputSystem controls;
    Vector2 startPos;
    Vector2 dragDir;
    //Vector2 swipe;
    float minSwipeSize = 10f;
    Direction swipeDirection;

    [SerializeField]
    CardHand hand;
    [SerializeField]
    SceneSwitcher sceneSwitcher;
    [SerializeField]
    Camera camera;
    int leftEdgeArea, rightEdgeArea;

    private void OnEnable()
    {
        Input.gyro.enabled = true;

        controls = new InputSystem();
        controls.Enable();

        #region mus controller
        //håller musens position uppdateras
        controls.test.MousePosition.performed +=  ctx => 
        { 
            UpdateMousePos(ctx.ReadValue<Vector2>());
        };
        //sparar musens start pos
        controls.test.MousePress.started += ctx => { SaveStartingPosMouse(); };
        //jämför musens position och väljer action
        controls.test.MousePress.canceled += ctx => { CompareThisPosToStartingPosMouse(); SwipeAction(ctx); };
        #endregion

        controls.test.KeyboardAny.canceled += SwipeActionKeyboard;
        controls.test.Tap.performed += ctx => { Debug.Log("Screen tap"); };
        //controls.test.Touch.enabled += ctx => { SaveStartingPos(ctx); }
        //controls.test.press.started += ctx => { SaveStartingPos(ctx.ReadValue<TouchState>().position); };
        controls.test.Swipe.performed += ctx =>
        {
            //Debug.Log("Continuous pos: " + ctx.ReadValue<Vector2>());
            GetSwipeDirection(ctx.ReadValue<Vector2>());

        };
        /*
        //ANTINGEN OVAN eller NEDAN, båda kan användas för att känna direction of swipe
        controls.test.press.performed += ctx => 
        {
            //Debug.Log("Area: " + ctx.ReadValue<TouchState>().position.x);
            SaveStartingPos(ctx.ReadValue<TouchState>());
            //Debug.Log("Magnitude: " + ctx.ReadValue<TouchState>().position.magnitude);
            //SaveStartingPosX(ctx.ReadValue<TouchState>().position.x);
            //CalculateTotalMagnitude(ctx.ReadValue<TouchState>().position.x);
            //GetSwipeDirection(ctx.ReadValue<TouchState>());
        };// GetSwipeDirection(ctx.ReadValue<TouchState>()); };// Debug.Log("Current pos: " + ctx.ReadValue<TouchState>().position + "  start pos: " + ctx.ReadValue<TouchState>().startPosition); };
        */
        controls.test.Area.performed += ctx =>
        {
            //Debug.Log("Area2: " + ctx.ReadValue<float>());
            SaveStartingPosX(ctx.ReadValue<float>());
        };
        //controls.test.press.WasPressedThisFrame += ctx => { Stuff(); };// SaveStartingPosX(ctx.ReadValue<TouchState>()); };
        //controls.test.Area.started += ctx => { Debug.Log("STARTED"); };
        controls.test.Area.canceled += ctx => { SetToZero(); };
        controls.test.Touch.canceled += ctx => { SwipeAction(ctx);};// 
        //controls.test.haspressedscreen. += ctx => { };

        //controls.test.press.canceled += SwipeAction;
        //controls.test.phonetest.performed += ctx => { Debug.Log(ctx.ReadValue<float>()); };
        //controls.test.PressingWithMouse.performed += ctx => { //ChooseAction();};
    }

    Vector2 mousePos;
    Vector2 mouseStartPos;
    private void UpdateMousePos(Vector2 pos)
    {
        mousePos = pos;
        //startPosX = pos.x;
    }
    private void SaveStartingPosMouse()
    {
        mouseStartPos = mousePos;
    }

    private void CompareThisPosToStartingPosMouse()
    {
        //compare mousePos and mouseStartPos
        Vector2 dir = mousePos - mouseStartPos;
        startPos2 = camera.ScreenToViewportPoint(new Vector2(mouseStartPos.x, 0f)).x;
        GetSwipeDirection(dir);
    }

    float startPosX;
    float startPos2;
    bool done = true;
    bool done2 = true;
    float totalMagnitude;

    private void Stuff()
    {
        //startPos2 = startPosX;
        //startPos2 = camera.ScreenToWorldPoint(new Vector2(startPosX, 0f)).x;
        startPos2 = camera.ScreenToViewportPoint(new Vector2(startPosX, 0f)).x;
        Debug.Log("Saved : " + startPos2);
    }
    private void SetToZero()
    {
        done = true;
        done2 = true;
        startPosX = 0f;
        //startPos = Vector2.zero;
        //Debug.Log("SetToZero");
    }
    private void SaveStartingPosX(float mag)
    {
        /*
        //if (startPosX == 0 && controls.test.Area.inProgress)
            //return;
        if (startPosX != 0)// && mag > startPosX)
            return;
        */

        startPosX = mag;
        if (controls.test.press.WasReleasedThisFrame())
            return;

        done2 = false;
    }
    private void CalculateTotalMagnitude(float mag)
    {
        totalMagnitude = Mathf.Abs(mag-startPosX);
    }
    /*
    private void SaveStartingPos(TouchState touch)
    {
        //only save when doesn't already have startPos
        //if (startPos != Vector2.zero)
        // return;
        if (!done)
            return;
        startPos = touch.startPosition;
        done = false;
        //Debug.Log(startPos);
    }
    */
    private void SwipeActionKeyboard(InputAction.CallbackContext c)
    {
        //Debug.Log("Swipe, direction: " + swipeDirection);
        SelectAction();
        //NOLLSTÄLL
        dragDir = Vector2.zero;
    }
    private void SwipeAction(InputAction.CallbackContext c)
    {
        //Debug.Log("Swipe, direction: " + swipeDirection);
        //Debug.Log("Size: " + (Mathf.Abs(dragDir.magnitude)) + " min size: " + minSwipeSize);
        //check if large enough touch
        if (Mathf.Abs(dragDir.magnitude) < minSwipeSize)
            return;
        SelectAction(); 
        //NOLLSTÄLL
        dragDir = Vector2.zero;
        //Debug.Log("Total mag dif: " + totalMagnitude);
    }
    private void SelectAction()
    {
        switch(swipeDirection)
        {
            case Direction.Up:
                //play card
                Debug.Log("Play card");
                hand.OldPlayCard();
                break;
            case Direction.Down:
                //open card deck
                //Debug.Log("Open card deck [PH]");
                hand.DrawCard();
                break;
            case Direction.Right:
                //either switch to garden scene or scroll cards
                //Debug.Log("R Used : " + startPos2);// + startPos.x + " old : " +startPosX);
                //Debug.Log("either switch to garden scene or scroll cards [PH] -->");
                
                
                if (startPos2 < 0.15f)
                {
                    //switch scene
                    Debug.LogError("Switch scene");
                    //sceneSwitcher.SwitchScene(-1); 
                }
                else
                {
                    //scroll cards
                    Debug.LogError("Scroll right");
                    //hand.ScrollRight();
                    hand.ShiftAllRight();
                }
                //Debug.Log("Startpos should be 0: " + startPos);
                
                SetToZero();
                break;
            case Direction.Left:
                //and.ShiftAllLeft();
                //Debug.Log("L Used : " + startPos2);//+ startPos.x + " old : " + startPosX);
                
                if (startPos2 > 0.85f)
                {
                    //switch scene
                    Debug.LogError("Switch scene");
                    //sceneSwitcher.SwitchScene(+1);
                }
                else
                {
                    //scroll cards
                    Debug.LogError("Scroll left");
                    //hand.ScrollLeft();
                    hand.ShiftAllLeft();
                }
                
                //Debug.Log("Startpos should be 0: " + startPos);
                SetToZero();
                break;
        }
    }



    private void GetSwipeDirection(TouchState touch)
    {
        startPos = touch.startPosition;
        //Debug.Log("STARTPOS: " +startPos);
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
        if(CheckForShake())
        {
            Debug.Log("SHAKE!");
        }
        //Debug.Log(Input.acceleration);// gyro.userAcceleration);
        if (UnityEngine.InputSystem.Gyroscope.current != null)
        {
            //Debug.Log(UnityEngine.InputSystem.Gyroscope.current.angularVelocity.ReadValue());
        }

        if(controls.test.haspressedscreen.triggered)
        {
            Debug.LogWarning("Triggered");
            Stuff();
        }
    }

    private bool CheckForShake()
    {
        Vector3 acceleration = Input.acceleration; //mobil rörelse
        return false;
    }

}
