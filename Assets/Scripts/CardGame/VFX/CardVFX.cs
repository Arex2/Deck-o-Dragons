using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardVFX : MonoBehaviour
{
    public delegate void Action(CardVFX cardVFX);

    public Card Card { get; private set; }
    public Target Target { get; private set; }

    public List<string> VFXTags => vfxTags;
    public float SpawnDelay => spawnDelay;
    public bool TriggersActions => triggersActions;

    [CardVFXTag(false)]
    [SerializeField] private List<string> vfxTags = new();

    [Space]
    [SerializeField] private bool triggersActions;
    [SerializeField] private float spawnDelay;
    [SerializeField] private float autoDestroyTimer;

    private ICardVFXComponent[] _vfxComponents;
    private bool _doingCoroutines;
    private int _totalCoroutineCount;
    private int _finishedCoroutineCount;

    private GameObject _root;

    private Action _action;
    private bool _triggeredAction;

    private void Awake()
    {
        _root = transform.root.gameObject;

        _vfxComponents = GetComponentsInChildren<ICardVFXComponent>(true);

        int vfxComponentsLength = _vfxComponents.Length;

        if (vfxComponentsLength > 0)
        {
            _doingCoroutines = true;
            _totalCoroutineCount = vfxComponentsLength;

            for (int i = 0; i < vfxComponentsLength; i++)
            {
                ICardVFXComponent vfxComponent = _vfxComponents[i];
                IEnumerator enumerator = vfxComponent.VFXCoroutine(this);

                if (enumerator == null)
                {
                    _finishedCoroutineCount++;
                }
                else
                {
                    StartCoroutine(DoCoroutine(enumerator));
                }
            }
        }
        else
        {
            DestroySelf(autoDestroyTimer);
        }
    }

    private void Start()
    {
        foreach (ICardVFXComponent vfxComponent in _vfxComponents)
        {
            vfxComponent.OnVFXCreated(this);
        }
    }

    private IEnumerator DoCoroutine(IEnumerator coroutine)
    {
        yield return coroutine;

        _finishedCoroutineCount++;
    }

    private void Update()
    {
        if (!_doingCoroutines)
        {
            return;
        }

        if (_finishedCoroutineCount < _totalCoroutineCount)
        {
            return;
        }

        DestroySelf();
    }

    public void DestroySelf(float delay = 0)
    {
        if (delay <= 0)
        {
            DestroySelfInstant();
        }
        else
        {
            StartCoroutine(DestroySelfDelayed(delay));
        }
    }

    private IEnumerator DestroySelfDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);

        DestroySelfInstant();
    }

    private void DestroySelfInstant()
    {
        if (triggersActions && !_triggeredAction)
        {
            TriggerAction();
        }

        foreach (ICardVFXComponent vfxComponent in _vfxComponents)
        {
            vfxComponent.OnVFXDestroyed(this);
        }

        CardVFXManager.OnDestroy(this);

        Destroy(_root);
    }

    public void SetCard(Card card) => Card = card;
    public void SetTarget(Target target) => Target = target;
    public void AddAction(Action action) => _action += action;

    #region SpawnVFX
    public List<CardVFX> SpawnVFX(IEnumerable<CardVFX> enumerable) => SpawnVFX(enumerable, Vector3.zero);
    public List<CardVFX> SpawnVFX(IEnumerable<CardVFX> enumerable, Vector3 position)
    {
        List<CardVFX> result = new();

        foreach (CardVFX vfx in enumerable)
        {
            result.Add(SpawnVFX(vfx, position));
        }

        return result;
    }

    public CardVFX SpawnVFX(CardVFX cardVFX) => SpawnVFX(cardVFX, Vector3.zero);
    public CardVFX SpawnVFX(CardVFX cardVFX, Vector3 position)
    {
        CardVFX vfx = CardVFXManager.SpawnVFX(cardVFX, position);

        if (vfx != null)
        {
            vfx.SetCard(Card);
            vfx.SetTarget(Target);
        }

        return vfx;
    }
    #endregion

    public void TriggerAction()
    {
        if (!triggersActions)
        {
            return;
        }

        _triggeredAction = true;

        _action?.Invoke(this);
    }
}
