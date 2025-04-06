using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void SwitchScene(int sceneIndexChange)
    {
        int newScenePos = SceneManager.GetActiveScene().buildIndex + sceneIndexChange;
        if (SceneManager.GetSceneByBuildIndex(newScenePos).IsValid())
            return;
        SceneManager.LoadScene(newScenePos);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(4);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
