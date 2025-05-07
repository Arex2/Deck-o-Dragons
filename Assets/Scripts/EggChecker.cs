using UnityEngine;

public class EggChecker : MonoBehaviour
{
    public GameObject panelButton;
    public GameObject meny;
    public GameObject block;
    public GameObject button;

    void Update()
    {
        GameObject egg = GameObject.FindWithTag("Egg");

        if (egg != null)
        {
            panelButton.SetActive(false);
            block.SetActive(true);
            meny.SetActive(false);
            button.SetActive(false);
        }
        else
        {
            panelButton.SetActive(true);
            block.SetActive(false);
            meny.SetActive(true);
            button.SetActive(true);
        }
    }
}
