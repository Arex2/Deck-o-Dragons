using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

public class RegisterClickWithMouse : MonoBehaviour
{
    bool mouseIsDown = false;
    RaycastHit2D hitLast;

    private void OnEnable()
    {
        InputSystem controls = new InputSystem();
        controls.CardMovement.Enable();
        //on mouse hold
        controls.CardMovement.Position.performed += ctx => { MyMouseHold(ctx.ReadValue<Vector2>()); };
        controls.CardMovement.Position1.performed += ctx => { MyMouseHold(ctx.ReadValue<Vector2>()); };

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
        mouseIsDown = true;

        Debug.Log("hitting");

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        hitLast = hit;
        if (hit)
        {
            // Call methods here
            Debug.Log("Raycast Hit -> " + hit.transform.name + " CLICK");

            if(hit.transform.CompareTag("Egg"))
            {
                Debug.Log("Egg being hit");
                hit.transform.GetComponent<Egg>().OnMouseDown();
            }


            if(hit.transform.CompareTag("Brush"))
            {
                hit.transform.GetComponent<BrushDragUI>().OnBeginDrag();
            }


            if (hit.transform.CompareTag("Food"))
            {
                hit.transform.GetComponent<DraggableUI>().OnBeginDrag();
            }


            if (hit.transform.CompareTag("Dragon"))
            {
                //hit.transform.GetComponent<DragonBehavior>().   metod som ska köras vid mouseClick
            }


        }
    }


    private void MyMouseHold(Vector2 mousePos)
    {
        if (!mouseIsDown)
            return;

        if (hitLast.transform == null)
            return;

        if (hitLast.transform.CompareTag("Brush"))
        {
            hitLast.transform.GetComponent<BrushDragUI>().OnDrag(mousePos);
        }

        if (hitLast.transform.CompareTag("Food"))
        {
            hitLast.transform.GetComponent<DraggableUI>().OnDrag(mousePos);
        }


        /*
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        if (hit)
        {
            // Call methods here
            Debug.Log("Raycast Hit -> " + hit.transform.name + " HOLD");

            if (hit.transform.name == "Egg")
            {
                //hit.transform.GetComponent<Egg>().OnMouseDown();
            }





        }*/
    }


    private void MyMouseExit()
    {
        if(hitLast.transform == null)
            return;


        if(hitLast.transform.CompareTag("Egg"))
        {
            Debug.Log("Raycast Hit -> " + hitLast.transform.name + " EXIT");
        }

        if (hitLast.transform.CompareTag("Brush"))
        {
            hitLast.transform.GetComponent<BrushDragUI>().OnEndDrag();
        }

        if (hitLast.transform.CompareTag("Food"))
        {
            hitLast.transform.GetComponent<DraggableUI>().OnEndDrag();
        }


        /*
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
        */


        mouseIsDown = false;

    }

}
