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

    private List<SearchTreeEntry> _searchTree = new();

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        _searchTree.Clear();

        _searchTree.Add(new SearchTreeGroupEntry(new GUIContent("Select Card Component"), 0));

        // Go through every Card Component type and add entries for each
        List<Entry> pendingEntries = new List<Entry>();

        foreach (Type type in Types)
        {
            Entry pendingEntry;

            // Take into account if the AddComponentMenu attribute is assigned
            AddComponentMenu addComponentMenu = type.GetCustomAttribute<AddComponentMenu>();

            if (addComponentMenu != null)
            {
                pendingEntry = new Entry(addComponentMenu, type);
            }
            else
            {
                pendingEntry = new Entry(type);
            }

            pendingEntries.Add(pendingEntry);
        }

        // Sort the entries based on their paths and order
        pendingEntries.Sort((a, b) =>
        {
            string[] splits1 = a.Path.Split('/');
            string[] splits2 = b.Path.Split('/');

            int split1Length = splits1.Length;
            int split2Length = splits2.Length;

            int compareValue = a.Order.CompareTo(b.Order);

            for (int i = 0; i < split1Length; i++)
            {
                if (i >= split2Length)
                {
                    return compareValue;
                }

                int value; //= splits1[i].CompareTo(splits2[i]);

                if (i == split1Length - 1)
                {
                    value = compareValue;
                }
                else
                {
                    value = splits1[i].CompareTo(splits2[i]);
                }

                if (value != 0)
                {
                    if (split1Length != split2Length && (i == split1Length - 1 || i == split2Length - 1))
                    {
                        return split1Length < split2Length ? 1 : -1;
                    }

                    return value;
                }
            }

            return compareValue;
        });

        List<string> groups = new List<string>();

        foreach (Entry pendingEntry in pendingEntries)
        {
            string[] entryTitle = pendingEntry.Path.Split('/');
            string groupName = "";
            int length = entryTitle.Length;

            for (int i = 0; i < length - 1; i++)
            {
                groupName += entryTitle[i];

                if (!groups.Contains(groupName))
                {
                    _searchTree.Add(new SearchTreeGroupEntry(new GUIContent(entryTitle[i]), i + 1));
                    groups.Add(groupName);
                }

                groupName += "/";
            }

            GUIContent content = new GUIContent(entryTitle[length - 1]);

            SearchTreeEntry treeEntry = new SearchTreeEntry(content);

            treeEntry.level = entryTitle.Length;
            treeEntry.userData = pendingEntry.Type;

            _searchTree.Add(treeEntry);
        }

        return _searchTree;

        /*
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
        */
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        OnSelectType?.Invoke((Type)SearchTreeEntry.userData);
        return true;
    }

    private class Entry
    {
        public string Path { get; private set; }
        public int Order { get; private set; }
        public Type Type { get; private set; }

        public Entry(string path, int order, Type type)
        {
            Path = path;
            Order = order;
            Type = type;
        }

        public Entry(AddComponentMenu addComponentMenu, Type type) : this(addComponentMenu.componentMenu, addComponentMenu.componentOrder, type) { }

        public Entry(Type type) : this(type.Name, 0, type) { }
    }
}