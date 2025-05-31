using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveDragons : MonoBehaviour
{
    /*
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
    */
}
