/// <summary>
/// <see cref="CardComponent"/> that will discard the players entire hand on use.
/// </summary>
// Script by Ruben
public class DiscardHand : CardComponent, IUse
{
    public void Use()
    {
        // TODO: This is a temporary solution for emptying the card hand
        CardHand hand = FindObjectOfType<CardHand>();

        hand.EmptyHand();
    }
}
