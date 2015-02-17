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

        list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Event Targets");
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

        //for (int i = 0; i < spellList.arraySize; i++)
        //{
        //    EditorGUILayout.BeginHorizontal("Box");
        //    Spell spell = spellList.GetArrayElementAtIndex(i).objectReferenceValue as Spell;
        //    if (spell == null)
        //    {
        //        spellList.DeleteArrayElementAtIndex(i);
        //        spellList.DeleteArrayElementAtIndex(i);
        //    }else
        //    EditorGUILayout.PropertyField(spellList.GetArrayElementAtIndex(i), new GUIContent(spell.spellID));
        //    if (GUILayout.Button(new GUIContent("[-]")))
        //    {
        //        spellList.DeleteArrayElementAtIndex(i);
        //        spellList.DeleteArrayElementAtIndex(i);
        //    }
        //    EditorGUILayout.EndHorizontal();
        //}

        //EditorGUILayout.BeginHorizontal("Box");
        //insertObj = EditorGUILayout.ObjectField(insertObj, typeof(Spell), false);
        //if (GUILayout.Button(new GUIContent("Insert")))
        //{
        //    int index = spellList.arraySize;
        //    spellList.InsertArrayElementAtIndex(index);
        //    SerializedProperty spellProp = spellList.GetArrayElementAtIndex(index);
        //    spellProp.objectReferenceValue = insertObj;
        //}
        //EditorGUILayout.EndHorizontal();

        list.DoLayoutList();

        serializedObject.ApplyModifiedProperties();

    }
}