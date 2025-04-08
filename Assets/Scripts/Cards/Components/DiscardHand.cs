public class DiscardHand : CardComponent, IUse
{
    public void Use()
    {
        // TODO: This is a temporary solution for emptying the card hand
        CardHand hand = FindObjectOfType<CardHand>();

        hand.EmptyHand();
    }
}
