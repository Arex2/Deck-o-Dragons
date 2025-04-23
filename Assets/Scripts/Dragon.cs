using UnityEngine;

public class Dragon : MonoBehaviour
{
    public GameObject nextStage;
    public int age = 1;

    private void Start()
    {
        FindObjectOfType<DragonEvolutionManager>().SetCurrentDragon(this);
    }
}
