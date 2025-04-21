using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Egg : MonoBehaviour
{
    private float timeBetweenShakes = 0.75f; //in seconds
    private float time = 0;
    //three stages of egg cracking
    //three shake instances needed
    private int shakeCount = 0;
    private bool isHatching;

    float shakeTreshold = 2.0f * 2.0f;
    Vector3 lowPassValue;

    [SerializeField] GameObject[] dragons;
    [SerializeField] private Sprite[] crackedSprites;
    private SpriteRenderer spriteRenderer;


    private void OnEnable()
    {
        Input.gyro.enabled = true;
    }

    void Start()
    {
        //typ default shake v�rde som alltid �r d�r (m�ng acceleration mobil naturligt har)
        lowPassValue = Input.acceleration;
        spriteRenderer = GetComponent<SpriteRenderer>();
        DragonActive.dragonActive = false;
        DragonActive.index = 0;
        DragonActive.age = 0;
    }

    void Update()
    {

        Vector3 acceleration = Input.acceleration;
        Vector3 deltaAcceleration = acceleration - lowPassValue;
        if(!isHatching && shakeCount == 3)
        {
            Hatch();
        } //only check this if shakeCounter != 3
        else if (!isHatching && deltaAcceleration.sqrMagnitude > shakeTreshold)
        {
            //shake cooldown timer
            time += Time.deltaTime;
            //shake egg while phone is shaking
            NewPos();

            if (time >= timeBetweenShakes)
            {
                Debug.Log("Shake!");
                //f�rg�ndring representerar sprite �ndring
                //d�r spriten f�r st�rre cracks
                if (shakeCount < crackedSprites.Length)
                {
                    spriteRenderer.sprite = crackedSprites[shakeCount];
                }
                shakeCount++;
                time = 0;
            }
        }

    }

    private void NewPos()
    {
        Vector3 acceleration = Input.acceleration;
        //ny position b�r vara x * shake direction
        //b�r bara flytta p� sig om shake �r �ver en viss punkt
        if(acceleration.magnitude > 1.5f)
        {
            transform.DOMove(acceleration * 0.5f, 0.1f);
        }
        transform.DOMove(new Vector3(0,0,0), 0.1f);
    }

    private void Hatch()
    {
        isHatching = true;
        SpawnDragon();
        Invoke("DeleteEgg",0.05f);
        //DeleteEgg();
    }

    public void SpawnEgg(GameObject egg)
    {
        Instantiate(egg, new Vector3(0, 0, 0), Quaternion.identity);
    }

    private void SpawnDragon()
    {
        int index = 1;  //Random.Range(0, dragons.Length);
        Instantiate(dragons[index], new Vector3(0, 0, 0), Quaternion.identity);
        //DragonActive.drPref = dragons[index];
        DragonActive.index = index;
        DragonActive.age = 1;
        DragonActive.dragonActive = true;
        Debug.Log(DragonActive.age);
    }

    private void DeleteEgg()
    {
        Destroy(gameObject);
    }

}