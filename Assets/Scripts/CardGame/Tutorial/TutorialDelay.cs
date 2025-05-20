using System.Collections;
using UnityEngine;

public class TutorialDelay : TutorialObject
{
    [SerializeField] private float delay;

    public override void Enable()
    {
        StartCoroutine(Delay());
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(delay);

        CardgameTutorialManager.ProgressTutorial();
    }

    public override void Disable()
    {

    }
}
