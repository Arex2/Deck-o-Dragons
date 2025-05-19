using System;
using UnityEngine;
using DG.Tweening;

public abstract class SceneSwitcherTransition : MonoBehaviour
{
    [CacheComponent(CacheMethod.InParent)]
    [SerializeField] private SceneSwitcher sceneSwitcher;

    [Space]
    [SerializeField] private SceneSwitcher.Scene[] scenes;

    public SceneSwitcher SceneSwitcher => sceneSwitcher;
    public SceneSwitcher.Scene[] Scenes => scenes;

    /// <summary>
    /// When the SceneSwitcher has registered this transition, this is called. Use this instead of Awake() or Start() for initial setup.
    /// </summary>
    public abstract void Initialize();

    /// <summary>
    /// Enables the transition visually. Is always called before <see cref="Appear"/>. Use this for setup for when the transition should start.
    /// </summary>
    public abstract void Enable();

    /// <summary>
    /// Disables the transition visually. Ensure that no element of the transition is visible when this is called.
    /// </summary>
    public abstract void Disable();

    /// <summary>
    /// Makes the transition appear on screen. Note that duration can be set to 0 or below, which means the transitions should instantly appear. <para/>
    /// Make sure you always call onFinish when the transition is finished appearing. It's recommended you use: <c>onFinish?.Invoke();</c> as it invokes without the possibility of null errors.
    /// </summary>
    public abstract void Appear(float duration, Ease ease = Ease.Unset, Action onFinish = null);

    /// <summary>
    /// Makes the transition disappear on screen. Note that duration can be set to 0 or below, which means the transitions should instantly disappear. <para/>
    /// Make sure you always call onFinish when the transition is finished disappearing. It's recommended you use: <c>onFinish?.Invoke();</c> as it invokes without the possibility of null errors.
    /// </summary>
    public abstract void Disappear(float duration, Ease ease = Ease.Unset, Action onFinish = null);
}
