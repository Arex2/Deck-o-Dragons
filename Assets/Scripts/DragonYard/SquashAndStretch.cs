using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquashAndStretch : MonoBehaviour
{
    [SerializeField] private Transform transformToAffect;
    [SerializeField] private SquashStretchAxis axisToAffect = SquashStretchAxis.Y;
    [SerializeField, Range(0, 1f)] private float animationDuration = 0.25f;
    [SerializeField] private bool canBeOverwritten;

    [Flags]
    public enum SquashStretchAxis
    {
        None = 0,
        X = 1,
        Y = 2,
        Z = 3
    }

    [SerializeField] private float initialScale = 1f;
    [SerializeField] private float maximumScale = 1.3f;
    [SerializeField] private bool resetToInitialScaleAfterAnimation = true;
    [SerializeField] private bool reverseAnimationCurveAfterPlaying;
    private bool isReversed;

    [SerializeField]
    private AnimationCurve squashAndStretchCurve = new AnimationCurve(
        new Keyframe(8f, 0f),
        new Keyframe(0.5f, 1f),
        new Keyframe(1f, 0f)
    );

    [SerializeField] private bool looping;
    [SerializeField] private float loopingDelay = 0.5f;

    private Coroutine squashAndStretchCoroutine;
    private WaitForSeconds loopingDelayWaitForSeconds;
    private Vector3 initialScaleVector;

    private bool affectX => (axisToAffect & SquashStretchAxis.X) != 0;
    private bool affectY => (axisToAffect & SquashStretchAxis.Y) != 0;
    private bool affectZ => (axisToAffect & SquashStretchAxis.Z) != 0;

    private void Awake()
    {
        if (transformToAffect == null)
        {
            transformToAffect = transform;

            initialScaleVector = transformToAffect.localScale;
            loopingDelayWaitForSeconds = new WaitForSeconds(loopingDelay);
        }
    }

    void Start()
    {
        CheckForAndStartCoroutine();
    }

    private void CheckForAndStartCoroutine()
    {
        if (axisToAffect == SquashStretchAxis.None)
        {
            Debug.Log("No axis to affect", gameObject);
            return;
        }

        if (squashAndStretchCoroutine != null)
        {
            StopCoroutine(squashAndStretchCoroutine);
            if (resetToInitialScaleAfterAnimation)
            {
                transform.localScale = initialScaleVector;
            }
        }

        squashAndStretchCoroutine = StartCoroutine(SquashAndStretchEffect());
    }

    public void PlaySquashAndStretch()
    {
        if(looping && !canBeOverwritten)
        {
            return;
        }

        CheckForAndStartCoroutine();
    }

    private IEnumerator SquashAndStretchEffect()
    {
        do
        {
            if(reverseAnimationCurveAfterPlaying)
            {
                isReversed = !isReversed;
            }

            float elapsedTime = 0;
            Vector3 originalScale = initialScaleVector;
            Vector3 modifiedScale = originalScale;

            while(elapsedTime < animationDuration)
            {
                elapsedTime += Time.deltaTime;

                float curvePosition;

                if(isReversed)
                {
                    curvePosition = 1 - (elapsedTime / animationDuration);
                }
                else
                {
                    curvePosition = elapsedTime / animationDuration;
                }

                //float curvePosition = elapsedTime / animationDuration;
                float curveValue = squashAndStretchCurve.Evaluate(curvePosition);
                float remappedValue = initialScale + (curveValue * (maximumScale - initialScale));

                float minimumThreshold = 0.0001f;
                if(Mathf.Abs(remappedValue) < minimumThreshold)
                {
                    remappedValue = minimumThreshold;
                }

                if(affectX)
                {
                    modifiedScale.x = originalScale.x * remappedValue;
                }
                else
                {
                    modifiedScale.x = originalScale.x / remappedValue;
                }

                if (affectY)
                {
                    modifiedScale.y = originalScale.y * remappedValue;
                }
                else
                {
                    modifiedScale.y = originalScale.y / remappedValue;
                }

                if (affectZ)
                {
                    modifiedScale.z = originalScale.z * remappedValue;
                }
                else
                {
                    modifiedScale.z = originalScale.z / remappedValue;
                }

                transformToAffect.localScale = modifiedScale;
                yield return null;
            }

            if(resetToInitialScaleAfterAnimation)
            {
                transformToAffect.localScale = originalScale;
            }

            if (looping)
            {
                yield return loopingDelayWaitForSeconds;
            }

        } while (looping);
    }

    public void SetLooping(bool shouldLoop)
    {
        looping = shouldLoop;
    }
}