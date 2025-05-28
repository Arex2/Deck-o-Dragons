using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "CardDeck", menuName = "Cards/Create New Card Deck", order = 20)]
public class CardDeck : ScriptableObject, IEnumerable<Card>
{
    [SerializeField] private Entry[] entries;

    public IEnumerator<Card> GetEnumerator()
    {
        foreach (Entry entry in entries)
        {
            for (int i = 0; i < entry.Copies; i++)
            {
                yield return entry.Card;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    [Serializable]
    private class Entry
    {
        public Card Card => card;
        public int Copies => copies;

        [SerializeField] private Card card;
        [SerializeField] private int copies = 1;

#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(Entry))]
        private class EntryPropertyDrawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                SerializedProperty cardProp = property.FindPropertyRelative(nameof(card));
                SerializedProperty copiesProp = property.FindPropertyRelative(nameof(copies));

                Rect copiesPosition = position;

                copiesPosition.xMin = copiesPosition.xMax - 50;

                position.xMax = copiesPosition.xMin - 10;

                EditorGUI.PropertyField(position, cardProp, GUIContent.none);
                EditorGUI.PropertyField(copiesPosition, copiesProp, GUIContent.none);
            }
        }
#endif
    }
}
