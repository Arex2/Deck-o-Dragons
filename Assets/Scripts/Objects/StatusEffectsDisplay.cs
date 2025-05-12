using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class StatusEffectsDisplay : MonoBehaviour, ITargetCallbacks
{
    [CacheComponent(CacheMethod.All)]
    [SerializeField] private Target target;

    [Space]
    [SerializeField] private StatusEffectsDisplayItem template;
    [SerializeField] private Transform parent;
    [SerializeField] private Transform inactiveParent;

    [Space]
    [SerializeField] private RectTransform layoutGroup;
    [SerializeField] private float spacing = 45;

    private Coroutine _rebuildCoroutine;

    private Dictionary<StatusEffectData, StatusEffectsDisplayItem> _activeItems = new();
    private ObjectPool<StatusEffectsDisplayItem> _pool;

    private HashSet<StatusEffectsDisplayItem> _itemsAddedThisFrame = new();

    private void Awake()
    {
        _pool = new(CreateObj);

        template.gameObject.SetActive(false);
    }

    private StatusEffectsDisplayItem CreateObj()
    {
        StatusEffectsDisplayItem newObj = Instantiate(template, parent);
        newObj.gameObject.SetActive(true);

        newObj.SetInactiveParent(inactiveParent);
        newObj.Disappear(true);

        return newObj;
    }

    private void LateUpdate()
    {
        bool rebuiltLayout = false;

        foreach (StatusEffectsDisplayItem item in _itemsAddedThisFrame)
        {
            if (!rebuiltLayout)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup);
            }

            item.Teleport();
        }

        if (rebuiltLayout)
        {
            _itemsAddedThisFrame.Clear();
        }
    }

    #region Unused Callbacks
    public void OnHurt() { }
    public void OnDeath() { }
    public void OnHeal() { }
    public void OnUpdateHP() { }
    public void OnTurnStart() { }
    public void OnTurnEnd() { }
    #endregion

    public void OnUpdateStatusEffects()
    {
        UpdateStatusEffectsDisplay();
    }

    private void UpdateStatusEffectsDisplay()
    {
        /*
        if (_rebuildCoroutine != null)
        {
            StopCoroutine(_rebuildCoroutine);
        }
        */

        HashSet<StatusEffectsDisplayItem> removeList = new(_activeItems.Values);
        _itemsAddedThisFrame.Clear();

        int index = 0;

        foreach (StatusEffectData statusEffectData in target.StatusEffects)
        {
            if (_activeItems.TryGetValue(statusEffectData, out StatusEffectsDisplayItem item))
            {
                item.SetData(statusEffectData);
                removeList.Remove(item);

                item.transform.SetSiblingIndex(index);
                index++;
                continue;
            }

            StatusEffect statusEffect = statusEffectData.StatusEffect;

            item = _pool.Get();
            item.SetData(statusEffectData);
            _activeItems.Add(statusEffectData, item);

            item.transform.SetSiblingIndex(index);
            index++;

            item.Appear();

            _itemsAddedThisFrame.Add(item);
        }

        foreach (StatusEffectsDisplayItem item in removeList)
        {
            _activeItems.Remove(item.Data);

            StatusEffectsDisplayItem capturedItem = item;
            item.Disappear(false, () => _pool.Release(capturedItem));
        }

        //_rebuildCoroutine = StartCoroutine(RebuildCoroutine());
    }

    /*
    private IEnumerator RebuildCoroutine()
    {
        for (int i = 0; i < 2; i++)
        {
            yield return null;
            Rebuild();
        }
    }

    private void Rebuild()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup);

        foreach (StatusEffectsDisplayItem item in _itemsAddedThisFrame)
        {
            item.Teleport();
        }
    }
    */
}
