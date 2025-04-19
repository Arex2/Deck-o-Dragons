using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// TODO: REMOVE
public abstract class TestTarget : Target
{
    [SerializeField] private TMP_Text hpTemp;
    private string _hpTempFormat;

    [SerializeField] private TMP_Text statusEffectsTemp;
    private string _statusEffectsTempFormat;

    protected override void Awake()
    {
        base.Awake();

        _hpTempFormat = hpTemp.text;
        UpdateHP();

        _statusEffectsTempFormat = statusEffectsTemp.text;
        UpdateStatusEffects();
    }

    protected override void UpdateHP()
    {
        hpTemp.text = string.Format(_hpTempFormat, hp.ToString());
    }

    protected override void UpdateStatusEffects()
    {
        string result = "";

        foreach (var pair in StatusEffectsData)
        {
            result += string.Format(_statusEffectsTempFormat, pair.Key.DisplayName, pair.Value.Potency, pair.Value.Duration) + "\n";
        }

        statusEffectsTemp.text = result;
    }

    public override Bounds GetWorldBounds()
    {
        return new Bounds(transform.position, transform.localScale);
    }
}
