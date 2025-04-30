using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadDragons : MonoBehaviour
{
    private DragonList dragonList;
    private void Awake()
    {
        LoadDragonsFromFile();
    }

    public void LoadDragonsFromFile()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "dragons.json");

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);

            dragonList = JsonUtility.FromJson<DragonList>(json);
        }
        foreach (Dragon dragon in dragonList.dragons)
        {
            DragonBookContents.SetNewDragonNameAndType(dragon.name, dragon.type);
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
