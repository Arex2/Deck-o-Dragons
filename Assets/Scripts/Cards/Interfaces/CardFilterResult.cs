/// <summary>
/// Delegate for filtering away certain <see cref="CardObject"/>s.
/// </summary>
public delegate CardFilterResult CardFilter(CardObject cardObject);

/// <summary>
/// The result of a <see cref="CardFilter"/> operation. <para/>
/// Use <see cref="Success"/> and <see cref="Failure"/> instead of any constructor.
/// </summary>
// Script by Ruben
public class CardFilterResult
{
    private static readonly CardFilterResult _instance = new();

    /// <summary>
    /// Use <see cref="Success"/> or <see cref="Failure"/> instead.
    /// </summary>
    private CardFilterResult() { }

    /// <summary>
    /// Returns a <see cref="CardFilterResult"/> that is successful.
    /// </summary>
    public static CardFilterResult Success()
    {
        _instance._failed = false;

        return _instance;
    }

    /// <summary>
    /// Returns a <see cref="CardFilterResult"/> that is unsuccessful and has a little <paramref name="message"/> about why it was unsuccessful.
    /// </summary>
    public static CardFilterResult Failure(string message)
    {
        _instance._failed = true;
        _instance._failMessage = message;

        return _instance;
    }

    /// <summary>
    /// If the Card Filter failed or not.
    /// </summary>
    public bool Failed => _failed;

    /// <summary>
    /// The message 
    /// </summary>
    public string FailMessage => _failMessage;

    private bool _failed;
    private string _failMessage;
}
