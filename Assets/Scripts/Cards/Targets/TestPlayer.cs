using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// TODO: REMOVE
public class TestPlayer : Target
{
    public override Team Team => Team.Player;

    [SerializeField] private TMP_Text hpTemp;
    private string _hpTempFormat;

    protected override void Awake()
    {
        base.Awake();

        _hpTempFormat = hpTemp.text;
        UpdateHP();
    }

    protected override void UpdateHP()
    {
        hpTemp.text = string.Format(_hpTempFormat, hp.ToString());
    }

    public override Bounds GetWorldBounds()
    {
        return new Bounds(transform.position, transform.localScale);
    }
}
