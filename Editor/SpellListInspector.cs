using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;

[CustomEditor(typeof(SpellList))]
public class SpellListInspector : Editor
{
    SerializedProperty spellList;
    Object insertObj;
    private ReorderableList list;


    void OnEnable()
    {
        spellList = serializedObject.FindProperty("spells");

        list = new ReorderableList(serializedObject, spellList, true, true, true, true);
        list.displayAdd = false;

        list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Spells");
        };

        list.drawElementCallback = ListDrawCallBack;

    }

    private void ListDrawCallBack(Rect rect, int index, bool isActive, bool isFocused)
    {
        var element = list.serializedProperty.GetArrayElementAtIndex(index);

        EditorGUI.PropertyField(
            new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
            element, GUIContent.none);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        list.DoLayoutList();
        insertObj = EditorGUILayout.ObjectField(insertObj, typeof(Spell));

        if (GUILayout.Button(new GUIContent("Insert Spell")) && insertObj != null)
        {
            if (!CanAddSpell(insertObj))
            {
                Debug.Log("Spell Already Exists");
                insertObj = null;
                return;
            }

            AddSpell(insertObj);
            insertObj = null;

        }

        if (GUILayout.Button("Try Load All Spells"))
            TryLoadAllSpells();

        serializedObject.ApplyModifiedProperties();

    }

    private void TryLoadAllSpells()
    {
        var spells = Resources.LoadAll("Prefabs/Spells/SpellImpl", typeof(Spell));
        foreach (Spell s in spells)
        {
            if (CanAddSpell(s))
                AddSpell(s);
            else
                continue;
            Debug.Log("Added Spell: " + s.spellID);
        }

    }

    private bool CanAddSpell(Object spell)
    {
        for (int i = 0; i < spellList.arraySize; i++)
        {
            if (spell == spellList.GetArrayElementAtIndex(i).objectReferenceValue)
            {
                return false;
            }
        }
        return true;
    }

    private void AddSpell(Object spell)
    {
        spellList.InsertArrayElementAtIndex(spellList.arraySize);
        SerializedProperty insert = spellList.GetArrayElementAtIndex(spellList.arraySize - 1);
        insert.objectReferenceValue = spell;
    }
}