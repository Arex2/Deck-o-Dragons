using UnityEngine;

public class EggChecker : MonoBehaviour
{
    public GameObject uiElement;
    public GameObject meny;
    public GameObject block;

    void Update()
    {
        GameObject egg = GameObject.FindWithTag("Egg");

        if (egg != null)
        {
            uiElement.SetActive(true);
            block.SetActive(true);
            meny.SetActive(false);
        }
        else
        {
            uiElement.SetActive(false);
            block.SetActive(false);
            meny.SetActive(true);
        }
    }
}
