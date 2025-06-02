using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

public enum Rotations
{
    Direction,
    Target,
    Spinn
}

public class AttackEffect : MonoBehaviour, ICardVFXComponent
{
    public bool dontRotate = false;


    public Rotations startRotationType = Rotations.Spinn;
    public Rotations endRotationType = Rotations.Direction;
    private Rotations rotationType;

    [SerializeField]
    Vector2 a, b, c, d; //a is startPos, d is targetPos


    [SerializeField]
    float desiredDuration = 1f;
    float percentageComplete;
    [SerializeField]
    Ease ease;


    /*
    [SerializeField]
    ParticleSystem explosion;
    */
    // Explosion is CardVFX instead
    [SerializeField]
    CardVFXReference deathVFX;

    private void Start()
    {
        rotationType = startRotationType;
    }

    // Update is called once per frame
    void Update()
    {
        //calculate next position
        Vector3 newPos = Bezier(a, b, c, d, percentageComplete);

        if(!dontRotate)
        {
            //switch rotation type on 50% complete
            if (percentageComplete > 0.5 && rotationType != endRotationType)
            {
                Debug.Log("Switched rotation");
                rotationType = endRotationType;
            }

            //rotate depending on selected rotation type
            switch (rotationType)
            {
                case Rotations.Spinn:
                    //rotation chaos
                    RotateChaosSpinn(newPos);
                    break;
                case Rotations.Target:
                    //projectile rotates to look at target
                    RotateToFaceTarget();
                    break;
                case Rotations.Direction:
                default:
                    //projectile rotates in movement direction
                    RotateTowardsDirection(newPos);
                    break;

            }
        }

        //move to next position
        transform.position = newPos;
    }


    public void OnVFXCreated(CardVFX cardVFX)
    {
        a = transform.position;
        b = a + new Vector2(Random.Range(-5, 5), Random.Range(-2, 2));
        c = a + new Vector2(Random.Range(-5, 5), Random.Range(2, 5));
        d = cardVFX.Target.GetHitPosition();
    }

    public IEnumerator VFXCoroutine(CardVFX cardVFX)
    {
        DOTween.To(() => percentageComplete, (value) => percentageComplete = value, 1, desiredDuration).SetEase(ease);

        yield return new WaitForSeconds(desiredDuration);

        // Instantiate explosion particle effect
        cardVFX.SpawnVFX(deathVFX, d);
    }

    public void OnVFXDestroyed(CardVFX cardVFX)
    {

    }

    /*
    public void DestroySelf()
    {
        //instantiate explosion particle effect
        ParticleSystem g = Instantiate(explosion);
        g.transform.position = d;
        //delete self
        Destroy(gameObject,0.1f);
    }
    */


    void RotateChaosSpinn(Vector3 dir)     //CHAOS
    {
        //makes projectiles spin wildly

        float angle = Mathf.Atan2((d.y - transform.position.y), (d.x - transform.position.x)) * Mathf.Rad2Deg;
        Quaternion lookRotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

        Vector3 diff = dir.normalized - transform.position.normalized;
        Quaternion targetRot = Quaternion.LookRotation(transform.forward, diff);
        Quaternion moveRotation = Quaternion.RotateTowards(transform.rotation, targetRot, 720 * Time.deltaTime);

        transform.rotation = lookRotation * moveRotation;
    }

    private void RotateToFaceTarget()
    {
        //projectile rotates to look at target
        float angle = Mathf.Atan2((d.y - transform.position.y), (d.x - transform.position.x)) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
    }

    private void RotateTowardsDirection(Vector3 dir)
    {
        //old version, un-smooth rotation
        /*
        Vector3 diff = dir.normalized - transform.position.normalized;
        Debug.Log(diff);
        Quaternion targetRot = Quaternion.LookRotation(transform.forward,diff);
        Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 720 * Time.deltaTime);

        transform.rotation = rotation;
        */

        //new version, smooth rotation towards move direction
        float angle2 = Mathf.Atan2((dir.y - transform.position.y), (dir.x - transform.position.x)) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle2 - 90));
    }

    //4-point Bezier curve
    //from: https://discussions.unity.com/t/how-to-get-a-smooth-curved-line-between-two-points-like-those-present-between-the-nodes-of-bolt-visual-scripting/246143
    Vector2 Bezier(Vector2 a, Vector2 b, float t)
    {
        return Vector2.Lerp(a, b, t);
    }

    Vector2 Bezier(Vector2 a, Vector2 b, Vector2 c, float t)
    {
        return Vector2.Lerp(Bezier(a, b, t), Bezier(b, c, t), t);
    }

    Vector2 Bezier(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t)
    {
        return Vector2.Lerp(Bezier(a, b, c, t), Bezier(b, c, d, t), t);
    }

}
