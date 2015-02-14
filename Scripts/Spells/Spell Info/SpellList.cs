using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellList : MonoBehaviour
{

    public Spell[] spells;

    public static SpellList Instance;

    private Dictionary<string, Spell> _spellDict;

    public void Awake()
    {
        Instance = this;
        _spellDict = new Dictionary<string, Spell>();

        foreach (Spell spell in spells)
        {
            if (!_spellDict.ContainsKey(spell.SpellID))
                _spellDict.Add(spell.SpellID, spell);
            else
                Debug.LogWarning("Spell already exists: " + spell.spellID);
            spell.gameObject.SetActive(false);
        }

    }

    public Spell GetNewSpell(string spell)
    {
        return  Instantiate(_spellDict[spell]);
    }

    public Spell GetNewSpell(Spell spell)
    {
        return GetNewSpell(spell.SpellID);
    }

    public Spell GetSpell(string spell)
    {
        return _spellDict[spell];
    }

    public Spell GetSpell(Spell spell)
    {
        return _spellDict[spell.SpellID];
    }

}

public enum SpellID{
    Fireball,
    Steam,
    PhysicalAttack,
    IceShard
}
