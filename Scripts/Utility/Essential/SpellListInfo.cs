using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellListInfo : ScriptableObject
{
    public Spell[] _spells;

    public Spell[] Spells
    {
        get { return _spells; }
    }

    public Dictionary<string, Spell> SpellDictionary
    {
        get
        {
            var spellDict = new Dictionary<string, Spell>();
            foreach (Spell spell in Spells)
            {
                if (!spellDict.ContainsKey(spell.SpellID))
                    spellDict.Add(spell.SpellID, spell);
                else
                    Debug.LogWarning("Spell already exists: " + spell.spellID);
                spell.gameObject.SetActive(false);
            }
            return spellDict;
        }
    }

}
