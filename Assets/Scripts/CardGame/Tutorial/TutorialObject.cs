using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TutorialObject : MonoBehaviour
{
    public HashSet<int> ActiveOn
    {
        get
        {
            if (_activeOnHashSet == null)
            {
                _activeOnHashSet = new();

                foreach (int i in activeOn)
                {
                    if (_activeOnHashSet.Contains(i))
                    {
                        continue;
                    }

                    _activeOnHashSet.Add(i);
                }
            }

            return _activeOnHashSet;
        }
    }
    private HashSet<int> _activeOnHashSet = null;

    public bool ActiveOnAll => activeOnAll;

    [SerializeField] private int[] activeOn;
    [SerializeField] private bool activeOnAll;

    private void OnEnable()
    {
        CardgameTutorialManager.TutorialObjects.Add(this);
    }

    private void OnDisable()
    {
        CardgameTutorialManager.TutorialObjects.Remove(this);
    }

    public abstract void Enable();

    public abstract void Disable();

    public virtual void OnTutorialOver()
    {

    }
}
