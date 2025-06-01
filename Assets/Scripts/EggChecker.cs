using UnityEngine;

public class EggChecker : MonoBehaviour
{
    public GameObject sword;
    public GameObject ball;
    public GameObject meatball;
    public GameObject meatball2;
    public GameObject meatball3;
    public GameObject meatball4;
    public GameObject brush;
    public GameObject settings;
    public GameObject bowl;
    public GameObject bowl2;
    public GameObject backyard;

    void Update()
    {
        GameObject egg = GameObject.FindWithTag("Egg");

        if (egg != null)
        {
            sword.SetActive(false);
            ball.SetActive(false);
            brush.SetActive(false);
            settings.SetActive(false);
            bowl.SetActive(false);
            bowl2.SetActive(false);
            meatball.SetActive(false);
            meatball2.SetActive(false);
            meatball3.SetActive(false);
            meatball4.SetActive(false);
            backyard.SetActive(false);
        }
        else
        {
            sword.SetActive(true);
            ball.SetActive(true);
            brush.SetActive(true);
            settings.SetActive(true);
            bowl.SetActive(true);
            bowl2.SetActive(true);
            meatball.SetActive(true);
            meatball2.SetActive(true);
            meatball3.SetActive(true);
            meatball4.SetActive(true);
            backyard.SetActive(true);

        }
    }
}
