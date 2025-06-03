using System;
using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEditor;

[SingletonMode(true)]
public class SaveManager : Singleton<SaveManager>
{
    public static string GetSavePath() => Path.Combine(Application.persistentDataPath, "save.txt");

    public static bool SeenWelcomePopup
    {
        get => _data.seenWelcomePopup;
        set => _data.seenWelcomePopup = value;
    }
    public static bool SeenBattleTutorial
    {
        get => _data.seenBattleTutorial;
        set => _data.seenBattleTutorial = value;
    }
    public static bool SeenEvolutionPopup
    {
        get => _data.seenEvolutionPopup;
        set => _data.seenEvolutionPopup = value;
    }
    public static bool SeenBackyardPopup
    {
        get => _data.seenBackyardPopup;
        set => _data.seenBackyardPopup = value;
    }

    public static bool MusicMuted
    {
        get => _data.musicMuted;
        set
        {
            _data.musicMuted = value;
            Instance.UpdateAudio();
        }
    }
    public static bool SFXMuted
    {
        get => _data.sfxMuted;
        set
        {
            _data.sfxMuted = value;
            Instance.UpdateAudio();
        }
    }

    public static List<int> DragonElementsGotten => _data.dragonElementsGotten;

    private static SaveData _data;

    [SerializeField] private AudioMixer audioMixer;

    private void Start()
    {
        UpdateAudio();
    }

    private void UpdateAudio()
    {
        audioMixer.SetFloat("MusicVolume", MusicMuted ? -80 : 0);
        audioMixer.SetFloat("SFXVolume", SFXMuted ? -80 : 0);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void Init()
    {
        Load();

        // Save every time a scene is loaded
        SceneManager.sceneLoaded += (scene, loadMode) => Save();
    }

    public static void Load()
    {
        if (!File.Exists(GetSavePath()))
        {
            _data = new SaveData();
        }
        else
        {
            _data = JsonUtility.FromJson<SaveData>(File.ReadAllText(GetSavePath()));
        }

        Load(_data);
    }

    private static void Load(SaveData data)
    {
        // Load current level
        ProgressManager.currentLevel = data.currentLevel;
        ProgressManager.wonLastBattle = data.wonLastBattle;

        // Load active dragon
        if (string.IsNullOrEmpty(data.activeDragonName))
        {
            DragonActive.dragonName = null;
            DragonActive.isDragonActive = false;
        }
        else
        {
            DragonActive.dragonName = data.activeDragonName;
            DragonActive.isDragonActive = true;
        }

        DragonActive.index = data.activeDragonIndex;
        DragonActive.age = data.activeDragonAge;
        DragonActive.evolutionProcess = data.activeDragonEvolutionProcess;
        DragonActive.doCheck = true;

        // Clear all dragons in book
        DragonBookContents.GetDragonNamesInBook().Clear();
        DragonBookContents.GetDragonTypesInBook().Clear();
        DragonBookContents.GetDragonsActiveInBackyard().Clear();
        DragonBookContents.GetElementsOfDragonsActiveInBackyard().Clear();

        // Load dragons into book
        foreach (SaveData.Dragon dragon in data.dragonsInBook)
        {
            DragonBookContents.SetNewDragonNameAndTypeInBook(dragon.name, dragon.index);
        }
        foreach (SaveData.Dragon dragon in data.dragonsActiveInBackyard)
        {
            DragonBookContents.SetNewDragonNamesAndElementsActiveInBackyard(dragon.name, dragon.index);
        }

        // Load cards
        if (data.blacklistedCardGuids.Count > 0)
        {
            DeckManager.Instance.BlackListCards.Clear();

            foreach (string cardGuid in data.blacklistedCardGuids)
            {
                Card card = CardManager.GetCardByGUID(cardGuid);

                if (card == null)
                {
                    continue;
                }

                DeckManager.Instance.BlackListCards.Add(card);
            }
        }
        if (data.cardGuids.Count > 0)
        {
            IEnumerable<Card> LoadCards()
            {
                foreach (string cardGuid in data.cardGuids)
                {
                    Card card = CardManager.GetCardByGUID(cardGuid);

                    if (card == null)
                    {
                        continue;
                    }

                    yield return card;
                }
            }

            DeckManager.Instance.InitializeDeck(LoadCards());
        }

        Instance.UpdateAudio();
    }

    public static void Save()
    {
        // Save current level
        _data.currentLevel = ProgressManager.currentLevel;
        _data.wonLastBattle = ProgressManager.wonLastBattle;

        // Save active dragon
        _data.activeDragonName = DragonActive.dragonName;
        _data.activeDragonIndex = DragonActive.index;
        _data.activeDragonAge =  DragonActive.age;
        _data.activeDragonEvolutionProcess = DragonActive.evolutionProcess;

        // Save dragons in book
        List<string> dragonNamesInBook = DragonBookContents.GetDragonNamesInBook();
        List<int> dragonTypessInBook = DragonBookContents.GetDragonTypesInBook();

        int length = Mathf.Min(dragonNamesInBook.Count, dragonTypessInBook.Count);

        _data.dragonsInBook.Clear();
        for (int i = 0; i < length; i++)
        {
            _data.dragonsInBook.Add(new SaveData.Dragon(dragonNamesInBook[i], dragonTypessInBook[i]));
        }

        // Save dragons in backyard
        List<string> dragonsActiveInBackyard = DragonBookContents.GetDragonsActiveInBackyard();
        List<int> elementsOfDragonsActiveInBackyard = DragonBookContents.GetElementsOfDragonsActiveInBackyard();

        length = Mathf.Min(dragonsActiveInBackyard.Count, elementsOfDragonsActiveInBackyard.Count);

        _data.dragonsActiveInBackyard.Clear();
        for (int i = 0; i < length; i++)
        {
            _data.dragonsActiveInBackyard.Add(new SaveData.Dragon(dragonsActiveInBackyard[i], elementsOfDragonsActiveInBackyard[i]));
        }

        // Save cards in deck
        _data.cardGuids.Clear();

        foreach (Card card in DeckManager.Instance.Deck)
        {
            _data.cardGuids.Add(card.GUID);
        }

        _data.blacklistedCardGuids.Clear();

        foreach (Card card in DeckManager.Instance.BlackListCards)
        {
            _data.blacklistedCardGuids.Add(card.GUID);
        }

        File.WriteAllText(GetSavePath(), JsonUtility.ToJson(_data));
    }

    public static void DeleteSave()
    {
        if (File.Exists(GetSavePath()))
        {
            File.Delete(GetSavePath());
        }

        SaveData newData = new();

        newData.musicMuted = _data.musicMuted;
        newData.sfxMuted = _data.sfxMuted;

        _data = newData;

        Load(_data);
    }

    [Serializable]
    private class SaveData
    {
        public bool seenWelcomePopup = false;
        public bool seenBattleTutorial = false;
        public bool seenEvolutionPopup = false;
        public bool seenBackyardPopup = false;

        public bool musicMuted = false;
        public bool sfxMuted = false;
        public int currentLevel = 0;
        public bool wonLastBattle = false;

        public string activeDragonName = null;
        public int activeDragonIndex = 0;
        public int activeDragonAge = 0;
        public int activeDragonEvolutionProcess = 0;

        public List<int> dragonElementsGotten = new();
        public List<Dragon> dragonsInBook = new();
        public List<Dragon> dragonsActiveInBackyard = new();
        public List<string> cardGuids = new();
        public List<string> blacklistedCardGuids = new();

        [Serializable]
        public class Dragon
        {
            public string name;
            public int index;

            public Dragon(string name, int index)
            {
                this.name = name;
                this.index = index;
            }
        }
    }
}
