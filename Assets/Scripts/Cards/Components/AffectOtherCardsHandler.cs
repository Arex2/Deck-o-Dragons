using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Affect Other Cards Handler")]
public class AffectOtherCardsHandler : CardComponent, IAffectOtherCardsHandler
{
    [SerializeField] private Optional<UpgradeableInt> count = new(true, new(3, default, 0, 0));
    [SerializeField] private Upgradeable<bool> requireExactAmount = new(false, 0, 0);

    [Space]
    [TextArea(1, 5)]
    [SerializeField] private string selectMessage;

    [Space]
    [SerializeField] private CardComponent[] triggerAfterSelect;

    public int? Count => count.Enabled ? count.Value[Level] : null;

    public bool RequireExactAmount => requireExactAmount[Level];

    public string SelectMessage => selectMessage;

    private IAffectOtherCards[] _affectOtherCards;

    public override void Initialize()
    {
        List<IAffectOtherCards> list = new();

        foreach (CardComponent cardComponent in triggerAfterSelect)
        {
            IAffectOtherCards affectOtherCards = cardComponent as IAffectOtherCards;

            if (affectOtherCards == null)
            {
                continue;
            }

            list.Add(affectOtherCards);
        }

        _affectOtherCards = list.ToArray();
    }

    public IEnumerator OnCardsSelected(List<CardObject> cardObjects)
    {
        foreach (IAffectOtherCards affectOtherCards in _affectOtherCards)
        {
            yield return affectOtherCards.OnCardsSelected(cardObjects);
        }
    }

    public CardFilterResult FilterCardObject(CardObject cardObject)
    {
        foreach (IAffectOtherCards affectOtherCards in _affectOtherCards)
        {
            CardFilterResult result = affectOtherCards.FilterCardObject(cardObject);

            if (result.Failed)
            {
                return result;
            }
        }

        return CardFilterResult.Success();
    }

    public override void Play(List<Target> targets)
    {

    }
}
