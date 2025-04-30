using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Egg : MonoBehaviour
{
    NativeKeyboardInputManager natInputMan;
    TextInputManager inputMan;
    DragonActive dragonActive;
    private float timeBetweenShakes = 0.75f; //in seconds
    private float time = 0;
    //three stages of egg cracking
    //three shake instances needed
    private int shakeCount = 0;
    private bool isHatching;

    float shakeTreshold = 2.0f * 2.0f;
    Vector3 lowPassValue;

    ScreenShake screenShake;

    //[SerializeField] GameObject[] dragons;
    [SerializeField] private Sprite[] crackedSprites;
    [SerializeField] GameObject eggInPieces;
    private SpriteRenderer spriteRenderer;
    private SquashAndStretch squashAndStretch;

    private void Awake()
    {
        dragonActive = GameObject.Find("DragonActive").GetComponent<DragonActive>();
    }

    private void OnEnable()
    {
        Input.gyro.enabled = true;
    }

    void Start()
    {
        //typ default shake v�rde som alltid �r d�r (m�ng acceleration mobil naturligt har)
        lowPassValue = Input.acceleration;
        spriteRenderer = GetComponent<SpriteRenderer>();
        DragonActive.isDragonActive = false;
        DragonActive.statusText.text = "";
        DragonActive.dragonName = null;
        DragonActive.index = 0;
        DragonActive.age = 0;
        DragonActive.evolutionProcess = 0;
        inputMan = GameObject.Find("Scripts").GetComponent<TextInputManager>();
        natInputMan = GameObject.Find("NativeInputManager").GetComponent<NativeKeyboardInputManager>();
        screenShake = GameObject.Find("Main Camera").GetComponent<ScreenShake>();
        squashAndStretch = gameObject.GetComponent<SquashAndStretch>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Hatch();
        }

        tapCurrent -= Time.deltaTime;
        if (tapCurrent < 0) tapCurrent = 0;
        //Debug.Log("tapCurrent: " + tapCurrent);

        Vector3 acceleration = Input.acceleration;
        Vector3 deltaAcceleration = acceleration - lowPassValue;
        if(!isHatching && shakeCount == 3)
        {
            //Hatch();
            StartCoroutine(WaitForHatch());
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

    private void OnMouseDown()
    {
        if (!isHatching && shakeCount >= 3)
        {
            //Hatch();
            StartCoroutine(WaitForHatch());
        }
        else
        {
            // Optional: tap counts as a shake if you want faster interaction
            Debug.Log("Egg tapped!");
            //ShakeByTap();
            TapToOpen();
        }
    }

    float tapThreshhold = 1f;
    float tapCurrent;

    private void TapToOpen()
    {
        //tap feedback effect
        squashAndStretch.PlaySquashAndStretch();


        //varje tap ökar tapCurrent;
        tapCurrent += 0.5f;
        //med tid sänks tapCurrent (i update)
        //om tapCurrent når över tapThreshold räknas det som 1 shake
        if(tapCurrent >= tapThreshhold)
        {
            //change sprite
            if (shakeCount < crackedSprites.Length)
            {
                spriteRenderer.sprite = crackedSprites[shakeCount];
            }

            //shake
            shakeCount++;
            //screenshake
            screenShake.StartShake();
            //reset tapCurrent
            tapCurrent = 0;
        }
    }

    #region old tap shake
    private void ShakeByTap()
    {
        time += timeBetweenShakes; // instantly "fills" the shake timer

        if (time >= timeBetweenShakes)
        {
            if (shakeCount < crackedSprites.Length)
            {
                spriteRenderer.sprite = crackedSprites[shakeCount];
            }

            shakeCount++;
            time = 0;

            if (shakeCount >= 3)
            {
                Hatch();
            }
        }
    }
    #endregion

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

        //TextInputManager.SpawnKeyboard();

        SpawnDragon();
        Invoke("DeleteEgg", 0.05f);
        //inputMan.SpawnKeyboard();
        natInputMan.OpenKeyboard();
        natInputMan.forCompUse = true;
        //Invoke("CallToOpenKeyboard", 1f);
        //DeleteEgg();
    }

    private void CallToOpenKeyboard()
    {
        natInputMan.OpenKeyboard();
    }

    private IEnumerator WaitForHatch()
    {
        isHatching = true;
        yield return new WaitForSeconds(0.3f);
        Hatch();
    }

    private void BreakOpenShell()
    {
        //spawna shellfragments på samma plats som ägget
        //lägg till fart på dessa fragments (i relation till hur mobilen hålls?)
        //och när de är utanför skärmen radera dem

        GameObject obj = Instantiate(eggInPieces);
        obj.transform.position = Vector3.zero;
    }

    public void SpawnEgg(GameObject egg)
    {
        //screenshake
        screenShake.StartShake();
        Instantiate(egg, new Vector3(0, 0, 0), Quaternion.identity);
    }

    private void SpawnDragon()
    {
        int index = UnityEngine.Random.Range(0, dragonActive.babyDragons.Length);
        Instantiate(dragonActive.babyDragons[index], new Vector3(0, -3, 0), Quaternion.identity);
        //DragonActive.drPref = dragons[index];
        DragonActive.index = index;
        DragonActive.age = 1;
        DragonActive.isDragonActive = true;
    }

    private void DeleteEgg()
    {
        BreakOpenShell();
        Destroy(gameObject);
    }
}