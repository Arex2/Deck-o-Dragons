using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CloudsTransition : MonoBehaviour
{
    private bool showClouds;
    private bool eggMoveRight;
    private bool eggMoveLeft;
    private bool yardMoveRight;
    private bool yardMoveLeft;

    // Start is called before the first frame update
    void Start()
    {
        if(showClouds)
        {
            showClouds = false;

            if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                transform.position = new Vector3(1027f, transform.position.y, transform.position.z);
                eggMoveLeft = true;

                /*do
                {
                    transform.Translate(-Vector3.left * 1f * Time.deltaTime);
                }
                while (transform.position.x < 1027);*/
            }

            if (SceneManager.GetActiveScene().buildIndex == 5)
            {
                transform.position = new Vector3(-772f, transform.position.y, transform.position.z);
                yardMoveLeft = true;

                /*while (transform.position.x < 1027)
                {
                    transform.Translate(-Vector3.right * 1f * Time.deltaTime);
                }*/
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (eggMoveRight)
        {
            transform.Translate(Vector3.right * 9000f * Time.deltaTime);

            if (transform.position.x >= 1027)
            {
                eggMoveRight = false;
            }
        }
        if (eggMoveLeft)
        {
            transform.Translate(Vector3.left * 9000f * Time.deltaTime);

            if (transform.position.x <= -3000)
            {
                eggMoveLeft = false;
            }
        }
        if (yardMoveRight)
        {
            transform.Translate(Vector3.right * 9000f * Time.deltaTime);

            if (transform.position.x >= 2251)
            {
                yardMoveRight = false;
            }
        }
        if (yardMoveLeft)
        {
            transform.Translate(Vector3.left * 9000f * Time.deltaTime);

            if (transform.position.x <= -772)
            {
                yardMoveLeft = false;
            }
        }
    }

    public void MoveToTheRight()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            showClouds = true;
            eggMoveRight = true;

            //while (transform.position.x < 1027)
            //{
                //transform.Translate(Vector3.right * 1000f * Time.deltaTime);
                //transform.position = Vector3.Lerp(new Vector3(-3000, 0, 0), new Vector3(1027, 0, 0), 1);
                //transform.position = Vector3.Lerp(transform.position, new Vector3(1027, 0, 0), Time.deltaTime * 20.0f);
            //}
        }

        if (SceneManager.GetActiveScene().buildIndex == 5)
        {
            yardMoveRight = true;

            /*while (transform.position.x < 1027)
            {
                transform.Translate(-Vector3.right * 1f * Time.deltaTime);
            }*/
        }
    }

    public void MoveToTheLeft()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            eggMoveLeft = true;

            /*while (transform.position.x < 3000)
            {
                transform.Translate(-Vector3.left * 100f * Time.deltaTime);
            }*/
        }

        if (SceneManager.GetActiveScene().buildIndex == 5)
        {
            showClouds = true;
            yardMoveLeft = true;

            /*while (transform.position.x < 1027)
            {
                transform.Translate(-Vector3.left * 1f * Time.deltaTime);
            }*/
        }
    }
}