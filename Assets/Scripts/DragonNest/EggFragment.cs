using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggFragment : MonoBehaviour
{
    Rigidbody2D rb;
    Vector2 direction;

    private void OnEnable()
    {
        Input.gyro.enabled = true;
    }

    //should move in the oposite direction from 0,0 (where the crack /or center should be)
    //and apply extra movement in gravity direction according to how the phone is held

    // Start is called before the first frame update
    void Start()
    {
        //add movement in direction
        //Input.gyro.enabled = true;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //depending on phone angle add direction to movement
        //transform.position = new Vector2(transform.position.x, transform.position.y);
        //Vector2 directionPhone = Input.acceleration;
        transform.rotation = Input.gyro.attitude;

        direction = transform.position - new Vector3(0,0,0); //center pos är 0,0,0
        //direction = direction * directionPhone;
        direction.Normalize();
    }

    private void FixedUpdate()
    {
        rb.velocity = direction * 4;
    }
}
