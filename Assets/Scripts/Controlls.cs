using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controlls : MonoBehaviour
{
    InputSystem controls;
    Vector2 swipeDir;

    private void OnEnable()
    {
        controls = new InputSystem();
        controls.Enable();

        controls.test.Swipe.performed += ctx =>
        {
            swipeDir = ctx.ReadValue<Vector2>();

        };
        controls.test.PressingWithMouse.performed += ctx => { ChooseAction(); };

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
        
        if(swipeDir.y > 0.6)
        {
            Debug.Log("UP");
        }
        else if (swipeDir.y < -0.6)
        {
            Debug.Log("DOWN");
        }
        else
        {
            if (swipeDir.x > 0)
            {
                Debug.Log("RIGHT");
                return;
            }
            else if (swipeDir.x < 0)
            {
                Debug.Log("LEFT");
                return;
            }
        }
    }
}
