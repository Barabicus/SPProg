using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(SpellList))]
public class SpellListInspector : Editor
{
    SerializedProperty spellList;
    Object insertObj;

    void OnEnable()
    {
         spellList = serializedObject.FindProperty("spells");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        List<int> deleteIndex = new List<int>();
        for (int i = 0; i < spellList.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal("Box");
            Spell spell = spellList.GetArrayElementAtIndex(i).objectReferenceValue as Spell;
            if (spell == null)
            {
                spellList.DeleteArrayElementAtIndex(i);
                spellList.DeleteArrayElementAtIndex(i);
            }else
            EditorGUILayout.PropertyField(spellList.GetArrayElementAtIndex(i), new GUIContent(spell.spellID));
            if (GUILayout.Button(new GUIContent("[-]")))
            {
                spellList.DeleteArrayElementAtIndex(i);
                spellList.DeleteArrayElementAtIndex(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal("Box");
        insertObj = EditorGUILayout.ObjectField(insertObj, typeof(Spell), false);
        if (GUILayout.Button(new GUIContent("Insert")))
        {
            int index = spellList.arraySize;
            spellList.InsertArrayElementAtIndex(index);
            SerializedProperty spellProp = spellList.GetArrayElementAtIndex(index);
            spellProp.objectReferenceValue = insertObj;
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

    }
}