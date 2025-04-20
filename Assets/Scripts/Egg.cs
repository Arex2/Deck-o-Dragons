using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
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
        //typ default shake värde som alltid är där (mäng acceleration mobil naturligt har)
        lowPassValue = Input.acceleration;
        spriteRenderer = GetComponent<SpriteRenderer>();
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
                //färgändring representerar sprite ändring
                //där spriten får större cracks
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
        //ny position bör vara x * shake direction
        //bör bara flytta på sig om shake är över en viss punkt
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

    private void SpawnDragon()
    {
        int index = Random.Range(0, dragons.Length);
        Instantiate(dragons[index], new Vector3(0, 0, 0), Quaternion.identity);
    }


    private void DeleteEgg()
    {
        Destroy(gameObject);
    }

}
