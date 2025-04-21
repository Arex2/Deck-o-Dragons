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

    public void SwitchToGarden()
    {
        if (SceneManager.GetSceneByBuildIndex(SceneManager.GetActiveScene().buildIndex) == SceneManager.GetSceneByBuildIndex(5))
            return;
        SceneManager.LoadScene(5);
    }

    public void SwitchToEgg()
    {
        if (SceneManager.GetSceneByBuildIndex(SceneManager.GetActiveScene().buildIndex) == SceneManager.GetSceneByBuildIndex(1))
            return;
        SceneManager.LoadScene(1);
        DragonActive.doCheck = true;
    }

    public void SwitchToCardGame()
    {
        if (SceneManager.GetSceneByBuildIndex(SceneManager.GetActiveScene().buildIndex) == SceneManager.GetSceneByBuildIndex(4))
            return;
        SceneManager.LoadScene(4);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(4);

        DeckManager.Instance.InitializeDeck(DeckManager.Instance.DefaultStarterDeck);
    }

    public void QuitGame()
    {
        Application.Quit();
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
