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
    public InputSystem controls;
    //Vector2 startPos;
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
    float leftEdgeArea = 0.15f;
    float rightEdgeArea = 0.85f;


    Vector2 mousePos;
    Vector2 mouseStartPos;

    float startPosX;
    float startPos2;

    private void OnEnable()
    {
        Input.gyro.enabled = true;

        controls = new InputSystem();
        //controls.Enable();


        #region mouse controls
        //håller musens position uppdateras
        controls.CardGame.MousePosition.performed +=  ctx => 
        { 
            UpdateMousePos(ctx.ReadValue<Vector2>());
        };
        //sparar musens start pos
        controls.CardGame.MousePress.started += ctx => { SaveStartingPosMouse(); };
        //jämför musens position och väljer action
        controls.CardGame.MousePress.canceled += ctx => { CompareThisPosToStartingPosMouse(); SwipeAction(ctx); };
        #endregion

        //controls.CardGame.KeyboardAny.canceled += SwipeActionKeyboard;

        controls.CardGame.Tap.performed += ctx => { Debug.Log("Screen tap"); };
        controls.CardGame.Swipe.performed += ctx =>
        {
            GetSwipeDirection(ctx.ReadValue<Vector2>());

        };
        controls.CardGame.Area.performed += ctx =>
        {
            SaveStartingPosX(ctx.ReadValue<float>());
        };
        controls.CardGame.Area.canceled += ctx => { SetToZero(); };
        controls.CardGame.Touch.canceled += ctx => { SwipeAction(ctx);}; 
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void UpdateMousePos(Vector2 pos)
    {
        mousePos = pos;
        //startPosX = pos.x;
    }
    private void SaveStartingPosMouse()
    {
        mouseStartPos = mousePos;
    }
    private void SaveStartingPosX(float mag)
    {
        startPosX = mag;
    }

    private void CompareThisPosToStartingPosMouse()
    {
        //compare mousePos and mouseStartPos
        Vector2 dir = mousePos - mouseStartPos;
        startPos2 = camera.ScreenToViewportPoint(new Vector2(mouseStartPos.x, 0f)).x;
        GetSwipeDirection(dir);
    }

    private void CaclulateScreenStartposX()
    {
        startPos2 = camera.ScreenToViewportPoint(new Vector2(startPosX, 0f)).x;
        Debug.Log("Saved : " + startPos2);
    }
    private void SetToZero()
    {
        startPosX = 0f;
    }

    /*
    private void SwipeActionKeyboard(InputAction.CallbackContext c)
    {
        SelectAction();
        //NOLLSTÄLL
        dragDir = Vector2.zero;
    }
    */
    float mag = 0;
    private void SwipeAction(InputAction.CallbackContext c)
    {

        #region swipe more than one at a time test

        //Debug.Log("MAGNITUDE: " + dragDir.magnitude);
        float size = Mathf.Abs(dragDir.x - startPos2);
        //Debug.Log("drag size: " + size);
        //mag = (int)(dragDir.magnitude/25);

        mag = (int)size/50;

        if (mag < 1) { mag = 1; }

        mag = 1;

        #endregion

        //check if large enough touch
        if (Mathf.Abs(dragDir.magnitude) < minSwipeSize)
            return;
        SelectAction(); 
        //NOLLSTÄLL
        dragDir = Vector2.zero;
    }
    private void SelectAction()
    {
        switch(swipeDirection)
        {
            case Direction.Up:
                //Debug.Log("Play card");
                hand.OldPlayCard();
                break;
            case Direction.Down:
                //hand.DrawCard();
                break;
            case Direction.Right:
                if (startPos2 < leftEdgeArea)
                {
                    Debug.Log("Switch scene");
                    //sceneSwitcher.SwitchScene(-1); 
                }
                else
                {
                    //Debug.Log("Scroll right");
                    for(int i = 0; i < mag; i++)
                        hand.ShiftAllRight();
                }
                SetToZero();
                break;
            case Direction.Left:
                if (startPos2 > rightEdgeArea)
                {
                    Debug.Log("Switch scene");
                    //sceneSwitcher.SwitchScene(+1);
                }
                else
                {
                    //Debug.Log("Scroll left");
                    for (int i = 0; i < mag; i++)
                        hand.ShiftAllLeft();
                }
                SetToZero();
                break;
        }
    }

    private void GetSwipeDirection(Vector2 touch)
    {
        dragDir = touch;
        //dragDir.Normalize();
        //Debug.Log(direction);
        //if x axis has more input(?) than y
        if (Mathf.Abs(dragDir.x) > Mathf.Abs(dragDir.y))
        {
            if (dragDir.x > 0) //RIGHT
            {
                swipeDirection = Direction.Right;
            }
            else //LEFT
            {
                swipeDirection = Direction.Left;
            }
        }
        else //y has more input
        {
            if (dragDir.y > 0) //UP
            {
                swipeDirection = Direction.Up;
            }
            else //DOWN
            {
                swipeDirection = Direction.Down;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (UnityEngine.InputSystem.Gyroscope.current != null)
        {
            //Debug.Log(UnityEngine.InputSystem.Gyroscope.current.angularVelocity.ReadValue());
        }

        if(controls.CardGame.haspressedscreen.triggered)
        {
            Debug.LogWarning("Triggered");
            CaclulateScreenStartposX();
        }
    }

    /*
    float totalMagnitude;
    private void CalculateTotalMagnitude(float mag)
    {
        totalMagnitude = Mathf.Abs(mag - startPosX);
    }
    */
    /*private void GetSwipeDirection(TouchState touch)
    {
        startPos = touch.startPosition;
        //Debug.Log("STARTPOS: " +startPos);
        //Vector3 AB = B - A.Destination - Origin.
        dragDir = touch.position - touch.startPosition;
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
    */

}
