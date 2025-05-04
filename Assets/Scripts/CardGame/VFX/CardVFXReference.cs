using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardVFXReference : IEnumerable<CardVFX>
{
    public CardVFX[] VFXs => vfxs;

    [CardVFXTag]
    [SerializeField] private string tag;
    [SerializeField] private CardVFX[] vfxs;

    private string _formattedTag;
    [NonSerialized]
    private bool _hasFormattedTag = false;

    public IEnumerator<CardVFX> GetEnumerator()
    {
        if (!string.IsNullOrEmpty(tag))
        {
            if (!_hasFormattedTag)
            {
                _hasFormattedTag = true;
                _formattedTag = CardVFXManager.FormatVFXTag(tag);
            }

            CardVFX[] cardVfxWithTag = CardVFXManager.GetVFXWithTag(_formattedTag, false);

            if (cardVfxWithTag != null)
            {
                foreach (CardVFX cardVfx in cardVfxWithTag)
                {
                    yield return cardVfx;
                }
            }
        }

        if (vfxs != null)
        {
            foreach (CardVFX cardVfx in vfxs)
            {
                yield return cardVfx;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public CardVFXReference(string tag)
    {
        this.tag = tag;
    }

    public override string ToString()
    {
        return $"CardVFXReference(Tag: \"{tag}\")";
    }
}
