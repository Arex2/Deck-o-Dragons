using UnityEngine;
using UnityEngine.InputSystem;

public class DraggableObject : MonoBehaviour
{
    private bool isDragging = false;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }
    public void OnTouchPress(InputValue value)
    {
        bool isPressed = value.isPressed;
        Debug.Log("isPressed: " + isPressed);

        if (isPressed)
        {
            Vector2 pointerPosition = Pointer.current.position.ReadValue();
            Vector2 worldPos = cam.ScreenToWorldPoint(new Vector3(pointerPosition.x, pointerPosition.y, cam.nearClipPlane));

            if (Vector2.Distance(transform.position, worldPos) < 0.5f) 
            {
                isDragging = true; 
                Debug.Log("Started dragging");
            }
        }
    }

    private void Update()
    {
        if (Pointer.current != null)
        {
            if (Pointer.current.press.isPressed)
            {
                if (!isDragging)
                {
                    Vector2 pointerPosition = Pointer.current.position.ReadValue();
                    Vector2 worldPos = cam.ScreenToWorldPoint(new Vector3(pointerPosition.x, pointerPosition.y, cam.nearClipPlane));

                    if (Vector2.Distance(transform.position, worldPos) < 4.5f) 
                    {
                        isDragging = true; 
                        Debug.Log("Started dragging");
                    }
                }
            }
            else
            {
                if (isDragging)
                {
                    isDragging = false;
                    Debug.Log("Released, stopped dragging");
                }
            }

            if (isDragging)
            {
                Vector2 pointerPosition = Pointer.current.position.ReadValue();
                Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(pointerPosition.x, pointerPosition.y, cam.nearClipPlane));

                transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);
            }
        }
    }
}
