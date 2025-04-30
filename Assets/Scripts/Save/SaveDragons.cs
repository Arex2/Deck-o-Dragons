using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveDragons : MonoBehaviour
{
    public static SaveDragons Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveDragonToFile(string name, int type)
    {
        Dragon dragon = new Dragon(name, type);

        string filePath = Path.Combine(Application.persistentDataPath, "dragons.json");

        DragonList dragonList = new DragonList();

        if (File.Exists(filePath))
        {
            // Read the JSON data from the file
            string json = File.ReadAllText(filePath);

            // If the file is empty, initialize the list with an empty array
            if (string.IsNullOrEmpty(json))
            {
                dragonList = new DragonList();
            }
            else
            {
                // Deserialize into DragonList
                dragonList = JsonUtility.FromJson<DragonList>(json);

                // Ensure the dragons list is initialized before adding a new dragon
                if (dragonList.dragons == null)
                {
                    dragonList.dragons = new List<Dragon>();
                }
            }
        }
        else
        {
            // If the file does not exist, create a new DragonList
            dragonList = new DragonList();
        }

        // Add the new dragon to the list
        dragonList.dragons.Add(dragon);

        // Convert the updated DragonList back to JSON
        string updatedJson = JsonUtility.ToJson(dragonList, true);

        // Save the updated JSON to the file
        File.WriteAllText(filePath, updatedJson);
    }

    [System.Serializable]
    public class Dragon
    {
        public string name;
        public int type;

        public Dragon(string name, int type)
        {
            this.name = name;
            this.type = type;
        }
    }

    [System.Serializable]
    public class DragonList
    {
        public List<Dragon> dragons = new List<Dragon>();  // Initialize dragons to avoid null
    }
}
