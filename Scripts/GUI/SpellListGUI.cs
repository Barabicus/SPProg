using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpellListGUI : MonoBehaviour
{
    public SpellList spellList;

    public Transform spellListBody;
    public Transform spellItemPrefab;
    public int spellsPerPage = 5;

    private List<Transform> _spellsInList = new List<Transform>();


    private void Start()
    {

    }

    #region Spell Methods

    public void CreateSpellList(string eType)
    {
        CreateSpellList((SpellElementType)Enum.Parse(typeof(SpellElementType), eType));
    }

    public void CreateSpellList(SpellElementType eType)
    {
        // Remove all spells in the list
        for (int i = _spellsInList.Count - 1; i >= 0; i--)
            RemoveSpell(_spellsInList[i]);

        int spellCount = 0;
        foreach (Spell s in spellList.spells)
        {
            if (s.elementType != eType)
                continue;
            AddSpell(s.spellName);
            spellCount++;
            if (spellCount == spellsPerPage)
                break;
        }
    }

    private void AddSpell(string name)
    {
        Transform t = Instantiate(spellItemPrefab);
        t.GetComponentInChildren<Text>().text = name;
        t.parent = spellListBody.transform;
        _spellsInList.Add(t);
    }

    private void RemoveSpell(Transform t)
    {
        _spellsInList.Remove(t);
        Destroy(t.gameObject);
    }

    #endregion

    #region Event Methods

    public bool MouseOver
    {
        set { GameplayGUI.instance.isMouseOver = value; }
    }

    #endregion

}
