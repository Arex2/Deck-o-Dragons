using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsBackyard : MonoBehaviour
{
    public bool swipeActions;

    public InputSystem controls;
    //Vector2 startPos;
    Vector2 dragDir;
    //Vector2 swipe;
    float minSwipeSize = 10f;
    Direction swipeDirection;

    [SerializeField]
    SceneSwitcher sceneSwitcher;
    [SerializeField]
    Camera camera;
    float leftEdgeArea = 0.20f;
    float rightEdgeArea = 0.80f;


    Vector2 mousePos;
    Vector2 mouseStartPos;

    float startPosX;
    float startPos2;

    private void OnEnable()
    {

        controls = new InputSystem();
        controls.Enable();

        #region mouse controls
        //h�ller musens position uppdateras
        controls.CardGame.MousePosition.performed += ctx =>
        {
            UpdateMousePos(ctx.ReadValue<Vector2>());
        };
        //sparar musens start pos
        controls.CardGame.MousePress.started += ctx => { SaveStartingPosMouse(); };
        //j�mf�r musens position och v�ljer action
        controls.CardGame.MousePress.canceled += ctx => { CompareThisPosToStartingPosMouse(); SwipeAction(ctx); };
        #endregion

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
        controls.CardGame.Touch.canceled += ctx => { SwipeAction(ctx); };
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void UpdateMousePos(Vector2 pos)
    {
        mousePos = pos;
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
    }
    private void SetToZero()
    {
        startPosX = 0f;
    }

    float mag = 0;
    private void SwipeAction(InputAction.CallbackContext c)
    {

        #region swipe more than one at a time test

        //Debug.Log("MAGNITUDE: " + dragDir.magnitude);
        float size = Mathf.Abs(dragDir.x - startPos2);
        //Debug.Log("drag size: " + size);
        //mag = (int)(dragDir.magnitude/25);

        mag = (int)size / 50;

        if (mag < 1) { mag = 1; }

        mag = 1;

        #endregion

        //check if large enough touch
        if (Mathf.Abs(dragDir.magnitude) < minSwipeSize)
            return;
        SelectAction();
        //NOLLST�LL
        dragDir = Vector2.zero;
    }
    private void SelectAction()
    {
        if (!swipeActions)
            return;

        switch (swipeDirection)
        {
            case Direction.Up:
                break;
            case Direction.Down:
                break;
            case Direction.Right:
                if (startPos2 < leftEdgeArea)
                {
                    Debug.Log("Switch scene");
                    //sceneSwitcher.SwitchScene(-1); 
                   // sceneSwitcher.SwitchToGarden();
                }
                SetToZero();
                break;
            case Direction.Left:
                if (startPos2 > rightEdgeArea)
                {
                    Debug.Log("Switch scene");
                    //sceneSwitcher.SwitchScene(+1);
                    sceneSwitcher.SwitchToEgg();
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
        if (controls.CardGame.haspressedscreen.triggered)
        {
            CaclulateScreenStartposX();
        }
    }

}
