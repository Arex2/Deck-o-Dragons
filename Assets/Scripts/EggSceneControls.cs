using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

enum EggSceneDirection
{
    Right, Left, Up, Down
}

public class EggSceneControls : MonoBehaviour
{
    public InputSystem controls;
    Vector2 dragDir;
    EggSceneDirection swipeDirection;

    float minSwipeSize = 10f;
    float leftEdgeArea = 0.20f;
    float rightEdgeArea = 0.80f;

    Vector2 touchStartPos;
    float startPos2;

    private void OnEnable()
    {
        Input.gyro.enabled = true;

        controls = new InputSystem();
        controls.Enable();

        // Capture starting position of the touch
        controls.CardGame.Touch.started += ctx =>
        {
            Vector2 start = Touchscreen.current.primaryTouch.position.ReadValue();
            touchStartPos = start;
            startPos2 = Camera.main.ScreenToViewportPoint(new Vector2(start.x, 0f)).x;
            // Debug.Log("Touch start viewport X: " + startPos2);
        };

        // Get swipe direction
        controls.CardGame.Swipe.performed += ctx =>
        {
            GetSwipeDirection(ctx.ReadValue<Vector2>());
        };

        // Trigger swipe action on release
        controls.CardGame.Touch.canceled += ctx =>
        {
            SwipeAction(ctx);
        };
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void SwipeAction(InputAction.CallbackContext ctx)
    {
        if (dragDir.magnitude < minSwipeSize)
            return;

        SelectAction();
        dragDir = Vector2.zero;
    }

    private void SelectAction()
    {
        switch (swipeDirection)
        {
            case EggSceneDirection.Right:
                if (startPos2 < leftEdgeArea)
                {
                    Debug.Log("Swipe right from left edge � switching to Garden");

                    // Can only switch if the player has at least one dragon in backyard
                    if (DragonBookContents.GetDragonNamesInBook().Count <= 0)
                    {
                        return;
                    }

                    SceneSwitcher.SwitchToGarden();
                }
                break;

            case EggSceneDirection.Left:
                if (startPos2 > rightEdgeArea)
                {
                    Debug.Log("Swipe left from right edge � switching to Card Game");
                   // SceneSwitcher.SwitchToCardGame();
                }
                break;

            // Optional: handle up/down if needed
            case EggSceneDirection.Up:
            case EggSceneDirection.Down:
                break;
        }
    }

    private void GetSwipeDirection(Vector2 swipe)
    {
        dragDir = swipe;

        if (Mathf.Abs(dragDir.x) > Mathf.Abs(dragDir.y))
        {
            swipeDirection = dragDir.x > 0 ? EggSceneDirection.Right : EggSceneDirection.Left;
        }
        else
        {
            swipeDirection = dragDir.y > 0 ? EggSceneDirection.Up : EggSceneDirection.Down;
        }
    }

    private void Update()
    {
        // Optional: debugging or visual feedback
    }
}