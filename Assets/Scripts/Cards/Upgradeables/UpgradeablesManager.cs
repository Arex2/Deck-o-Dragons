using System.Text;
using UnityEngine;

[SingletonMode(true)]
public class UpgradeablesManager : Singleton<UpgradeablesManager>
{
    public static Color UpgradedColor => Instance.upgradedColor;
    public static Color DowngradedColor => Instance.downgradedColor;

    public static string UpgradedColorHTML
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_upgradedColorHTML))
            {
                _upgradedColorHTML = ColorUtility.ToHtmlStringRGBA(UpgradedColor);
            }

            return _upgradedColorHTML;
        }
    }
    private static string _upgradedColorHTML;
    public static string DowngradedColorHTML
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_downgradedColorHTML))
            {
                _downgradedColorHTML = ColorUtility.ToHtmlStringRGBA(DowngradedColor);
            }

            return _downgradedColorHTML;
        }
    }
    private static string _downgradedColorHTML;

    [SerializeField] private Color upgradedColor = Color.green;
    [SerializeField] private Color downgradedColor = Color.red;

    private static readonly StringBuilder _stringBuilder = new();
    private const string COLOR_PREFIX = "<color=#";
    private const string COLOR_PREFIX_END = ">";
    private const string COLOR_SUFFIX = "</color>";

    public static string ColorBasedOnLevel(string text, int level)
    {
        if (level == 0)
        {
            return text;
        }

        _stringBuilder.Clear();

        _stringBuilder.Append(COLOR_PREFIX);

        if (level > 0)
        {
            _stringBuilder.Append(UpgradedColorHTML);
        }
        else
        {
            _stringBuilder.Append(DowngradedColorHTML);
        }

        _stringBuilder.Append(COLOR_PREFIX_END);

        _stringBuilder.Append(text);

        _stringBuilder.AppendLine(COLOR_SUFFIX);

        return _stringBuilder.ToString();
    }
}
