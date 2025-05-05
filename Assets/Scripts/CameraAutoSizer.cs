using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject background; // Dra bakgrundens GameObject hit i inspector

    void Start()
    {
        if (background != null)
        {
            // Beräkna kamerans ortografiska storlek baserat på bakgrundens bredd
            SetCameraSize();
        }
        else
        {
            Debug.LogError("Bakgrund är inte tilldelad! Se till att du drar bakgrundens GameObject till skriptet.");
        }
    }

    void SetCameraSize()
    {
        // Hämta bakgrundens SpriteRenderer
        SpriteRenderer backgroundRenderer = background.GetComponent<SpriteRenderer>();

        if (backgroundRenderer != null)
        {
            // Hämta bakgrundens bredd i world units
            float backgroundWidth = backgroundRenderer.bounds.size.x;

            // Hämta kamerans aspect ratio
            float aspectRatio = (float)Screen.width / (float)Screen.height;

            // Debugga värden för att kolla om allt är korrekt
            Debug.Log("Bakgrundens bredd: " + backgroundWidth);
            Debug.Log("Aspect ratio: " + aspectRatio);

            // Beräkna kamerans ortografiska storlek så att bredden alltid matchar bakgrundens bredd
            Camera.main.orthographicSize = backgroundWidth / (2f * aspectRatio);

            // Debugga den nya ortografiska storleken
            Debug.Log("Ny ortografisk storlek: " + Camera.main.orthographicSize);
        }
        else
        {
            Debug.LogError("Bakgrundens SpriteRenderer hittades inte. Kontrollera att objektet har en SpriteRenderer.");
        }
    }
}
