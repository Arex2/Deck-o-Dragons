using UnityEngine;

public class CardVFXTagAttribute : PropertyAttribute
{
    public bool ShowNoTagOption => _showNoTagOption;

    private bool _showNoTagOption;

    public CardVFXTagAttribute(bool showNoTagOption = true)
    {
        _showNoTagOption = showNoTagOption;
    }
}