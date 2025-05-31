using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class CardCopyData : IEnumerable<Card>
{
    public Card Card => card;
    public int Copies => copies;

    [SerializeField] private Card card;
    [SerializeField] private int copies = 1;

    public IEnumerator<Card> GetEnumerator()
    {
        for (int i = 0; i < copies; i++)
        {
            yield return card;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(CardCopyData))]
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
