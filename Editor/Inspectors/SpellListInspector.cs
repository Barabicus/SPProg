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
            for (int i = 0; i < spellList.arraySize; i++)
            {
                if (insertObj == spellList.GetArrayElementAtIndex(i).objectReferenceValue)
                {
                    Debug.Log("Spell Already Exists");
                    insertObj = null;
                    return;
                }
            }

                spellList.InsertArrayElementAtIndex(spellList.arraySize);
            SerializedProperty insert = spellList.GetArrayElementAtIndex(spellList.arraySize - 1);
            insert.objectReferenceValue = insertObj;
            insertObj = null;
        }

        serializedObject.ApplyModifiedProperties();

    }
}