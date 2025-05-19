using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CloudsTransition : MonoBehaviour
{
    private bool showClouds;

    // Start is called before the first frame update
    void Start()
    {
        if(showClouds)
        {
            showClouds = false;

            if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                while (transform.position.x < 1027)
                {
                    transform.Translate(-Vector3.left * 1f * Time.deltaTime);
                }
            }

            if (SceneManager.GetActiveScene().buildIndex == 5)
            {
                while (transform.position.x < 1027)
                {
                    transform.Translate(-Vector3.right * 1f * Time.deltaTime);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveToTheRight()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            showClouds = true;

            while (transform.position.x < 1027)
            {
                transform.Translate(-Vector3.right * 500f * Time.deltaTime);
            }
        }

        if (SceneManager.GetActiveScene().buildIndex == 5)
        {
            while (transform.position.x < 1027)
            {
                transform.Translate(-Vector3.right * 1f * Time.deltaTime);
            }
        }
    }

    public void MoveToTheLeft()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            while (transform.position.x < 3000)
            {
                transform.Translate(-Vector3.left * 100f * Time.deltaTime);
            }
        }

        if (SceneManager.GetActiveScene().buildIndex == 5)
        {
            showClouds = true;

            while (transform.position.x < 1027)
            {
                transform.Translate(-Vector3.left * 1f * Time.deltaTime);
            }
        }
    }
}