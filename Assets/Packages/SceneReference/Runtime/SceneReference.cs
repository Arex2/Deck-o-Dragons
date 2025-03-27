using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

/// <summary>
/// A Serializable reference to a Scene <see cref="Object"/>.
/// </summary>
[Serializable]
public class SceneReference
{
    /// <summary>
    /// Returns if this Scene is a valid scene in the build settings.
    /// </summary>
    public bool IsValidScene => BuildIndex >= 0;

    /// <summary>
    /// The name of the Scene this references.
    /// </summary>
    public string SceneName => sceneName;
    [SerializeField] private string sceneName = string.Empty;

    /// <summary>
    /// The asset path of the Scene this references.
    /// </summary>
    public string ScenePath => scenePath;
    [SerializeField] private string scenePath = string.Empty;

    /// <summary>
    /// The build index of the Scene this references. Will be -1 if this scene is not in the build settings. <br/>
    /// See also: <see cref="IsValidScene"/>.
    /// </summary>
    public int BuildIndex => buildIndex;
    [SerializeField] private int buildIndex = -1;

#if UNITY_EDITOR
    [SerializeField] private Object sceneAsset;
#endif

    public static implicit operator int(SceneReference sceneReference) => sceneReference.BuildIndex;

    public override string ToString()
    {
        return $"{SceneName} ({buildIndex})";
    }

    /// <summary>
    /// Loads this <see cref="SceneReference"/>.
    /// </summary>
    public void Load(LoadSceneMode loadSceneMode = LoadSceneMode.Single)
    {
        SceneManager.LoadScene(buildIndex, loadSceneMode);
    }

    /// <summary>
    /// Loads this <see cref="SceneReference"/> asynchronously.
    /// </summary>
    public AsyncOperation LoadAsync(LoadSceneMode loadSceneMode = LoadSceneMode.Single)
    {
        return SceneManager.LoadSceneAsync(buildIndex, loadSceneMode);
    }
}