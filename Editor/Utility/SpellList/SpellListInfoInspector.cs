using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(SpellListInfo))]
public class SpellListInfoInspector : Editor {

    private SerializedProperty _spellList;
    private Object _insertObj;
    private ReorderableList _list;


    void OnEnable()
    {
        _spellList = serializedObject.FindProperty("_spells");

        _list = new ReorderableList(serializedObject, _spellList, true, true, true, true);
        _list.displayAdd = false;

        _list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Spells");
        };

        _list.drawElementCallback = ListDrawCallBack;
    }

    private void ListDrawCallBack(Rect rect, int index, bool isActive, bool isFocused)
    {
        var element = _list.serializedProperty.GetArrayElementAtIndex(index);

        EditorGUI.PropertyField(
            new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
            element, GUIContent.none);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        _list.DoLayoutList();
        _insertObj = EditorGUILayout.ObjectField(_insertObj, typeof(Spell));

        if (GUILayout.Button(new GUIContent("Insert Spell")) && _insertObj != null)
        {
            if (!CanAddSpell(_insertObj))
            {
                Debug.Log("Spell Already Exists");
                _insertObj = null;
                return;
            }

            AddSpell(_insertObj);
            _insertObj = null;

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
        for (int i = 0; i < _spellList.arraySize; i++)
        {
            if (spell == _spellList.GetArrayElementAtIndex(i).objectReferenceValue)
            {
                return false;
            }
        }
        return true;
    }

    private void AddSpell(Object spell)
    {
        _spellList.InsertArrayElementAtIndex(_spellList.arraySize);
        SerializedProperty insert = _spellList.GetArrayElementAtIndex(_spellList.arraySize - 1);
        insert.objectReferenceValue = spell;
    }
}
