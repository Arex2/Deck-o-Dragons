using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

/// <summary>
/// Editor script that provides a search window with every <see cref="CardComponent"/> in the game.
/// </summary>
// Script by Ruben
public class CardComponentSearchWindowProvider : ScriptableObject, ISearchWindowProvider
{
    public Action<Type> OnSelectType { get; set; }

    public static List<Type> Types
    {
        get
        {
            if (_cachedTypes == null)
            {
                _cachedTypes = new List<Type>();

                // Loop through every type in the entire app
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (!typeof(CardComponent).IsAssignableFrom(type) || type.IsAbstract)
                        {
                            continue;
                        }

                        _cachedTypes.Add(type);
                    }
                }
            }

            return _cachedTypes;
        }
    }
    private static List<Type> _cachedTypes = null;

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        // Create a new search tree because our current one is nonexistent
        List<SearchTreeEntry> searchTree = new List<SearchTreeEntry>
            {
                // Add the top title to the search tree
                new SearchTreeGroupEntry(new GUIContent($"Select Card Component"), 0)
            };

        foreach (Type type in Types)
        {
            GUIContent content = new GUIContent(type.Name, EditorGUIUtility.ObjectContent(null, type).image);
            SearchTreeEntry entry = new SearchTreeEntry(content);

            entry.level = 1;
            entry.userData = type;

            searchTree.Add(entry);
        }

        return searchTree;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        OnSelectType?.Invoke((Type)SearchTreeEntry.userData);
        return true;
    }
}