using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadDragons : MonoBehaviour
{
    private DragonList dragonList;
    public static LoadDragons Instance { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadDragonsFromFile();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadDragonsFromFile()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "dragons.json");

        if (!File.Exists(filePath))
        {
            dragonList = new DragonList();
            string emptyJson = JsonUtility.ToJson(dragonList, true);
            File.WriteAllText(filePath, emptyJson);
        }
        string json = File.ReadAllText(filePath);
        dragonList = JsonUtility.FromJson<DragonList>(json);

        foreach (Dragon dragon in dragonList.dragons)
        {
            DragonBookContents.SetNewDragonNameAndTypeInBook(dragon.name, dragon.type);
        }
    }

    [System.Serializable]
    public class Dragon
    {
        public string name;
        public int type;
    }

    [System.Serializable]
    public class DragonList
    {
        public List<Dragon> dragons;
    }

}
