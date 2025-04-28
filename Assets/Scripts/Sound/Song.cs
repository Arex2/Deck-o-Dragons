using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Song", menuName = "Song", order = 220)]
public class Song : ScriptableObject
{
    public bool Loaded
    {
        get
        {
            foreach (SongLayer layer in layers)
            {
                if (!layer.Loaded)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public SongLayer[] Layers => layers;

    [SerializeField] private SongLayer[] layers;
    [SerializeField] private Style[] styles;

    private Dictionary<string, Style> _styleDictionary = null;

    public List<SongLayer> GetStyleLayers(string name)
    {
        if (_styleDictionary == null)
        {
            _styleDictionary = new Dictionary<string, Style>();

            foreach (Style style in styles)
            {
                _styleDictionary.Add(style.Name.ToLower().Trim(), style);
            }
        }

        if (!_styleDictionary.TryGetValue(name.ToLower().Trim(), out Style result))
        {
            return null;
        }

        return result.Layers;
    }

    public void Load()
    {
        foreach (SongLayer layer in layers)
        {
            layer.Load();
        }
    }

    public void Unload()
    {
        foreach (SongLayer layer in layers)
        {
            layer.Unload();
        }
    }

    [Serializable]
    public class Style
    {
        public string Name => name;
        public List<SongLayer> Layers => layers;

        [SerializeField] private string name = "layer";
        [SerializeField] private List<SongLayer> layers;
    }
}
