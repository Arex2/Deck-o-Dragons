using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

public class RegisterClickWithMouse : MonoBehaviour
{
    private void OnEnable()
    {
        InputSystem controls = new InputSystem();
        controls.CardMovement.Enable();
        //on mouse hold
        controls.CardMovement.Position.performed += ctx => { MyMouseHold(); };
        controls.CardMovement.Position1.performed += ctx => { MyMouseHold(); };

        //on mouse down
        controls.CardMovement.Touch.performed += ctx => { MyMouseClick(); };
        //on mouse exit
        controls.CardMovement.Contact.canceled += ctx => { MyMouseExit(); };

        //controls.CardMovement.Position.performed += ctx => { OnContact(ctx.ReadValue<Vector2>()); };
        //controls.CardMovement.Direction.performed += ctx => { dragDir = ctx.ReadValue<Vector2>(); };

        //controls.CardMovement.Contact.canceled += ctx => { OnRelease(); };
    }

    //från: https://discussions.unity.com/t/onmousedown-with-new-input-system/805305/9
    private void MyMouseClick()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hit)
        {
            // Call methods here
            Debug.Log("Raycast Hit -> " + hit.transform.name);

            if(hit.transform.name == "Egg")
            {
                hit.transform.GetComponent<Egg>().OnMouseDown();
            }
        }
    }


    private void MyMouseHold()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hit)
        {
            // Call methods here
            Debug.Log("Raycast Hit -> " + hit.transform.name);

            if (hit.transform.name == "Egg")
            {
                //hit.transform.GetComponent<Egg>().OnMouseDown();
            }


        }
    }


    private void MyMouseExit()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hit)
        {
            // Call methods here
            Debug.Log("Raycast Hit -> " + hit.transform.name   + " EXIT");

            if (hit.transform.name == "Egg")
            {
                //hit.transform.GetComponent<Egg>().OnMouseDown();
            }


        }
    }

}
