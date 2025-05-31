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
            string json = File.ReadAllText(filePath);

            if (string.IsNullOrEmpty(json))
            {
                dragonList = new DragonList();
            }
            else
            {
                dragonList = JsonUtility.FromJson<DragonList>(json);

                if (dragonList.dragons == null)
                {
                    dragonList.dragons = new List<Dragon>();
                }
            }
        }
        else
        {
            dragonList = new DragonList();
        }

        dragonList.dragons.Add(dragon);

        string updatedJson = JsonUtility.ToJson(dragonList, true);

        File.WriteAllText(filePath, updatedJson);
    }

    public void SaveActiveDragonToFile(string dragonName, int dragonType, int level, bool hasDoneTutorial)
    {
        SaveVariables saveVariables = new SaveVariables(dragonName, dragonType, level, hasDoneTutorial);
        string filePath = Path.Combine(Application.persistentDataPath, "saveVariables.json");

        string json = JsonUtility.ToJson(saveVariables, true);

        File.WriteAllText(filePath, json);

        
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
        public List<Dragon> dragons = new List<Dragon>();  
    }

    [System.Serializable]
    public class SaveVariables
    {
        public string dragonName;
        public int dragonType;
        public int level;
        public bool hasDoneTutorial;

        public SaveVariables(string dragonName, int dragonType, int level, bool hasDoneTutorial)
        {
            this.dragonName = dragonName;
            this.dragonType = dragonType;
            this.level = level;
            this.hasDoneTutorial = hasDoneTutorial;
        }
    }
}
