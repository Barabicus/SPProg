using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellList : MonoBehaviour
{

    public Spell[] spells;

    public static SpellList Instance;

    private Dictionary<SpellID, Spell> _spellDict;

    public void Awake()
    {
        Instance = this;
        _spellDict = new Dictionary<SpellID, Spell>();

        foreach (Spell spell in spells)
        {
            if (!_spellDict.ContainsKey(spell.SpellID))
                _spellDict.Add(spell.SpellID, spell);
            spell.gameObject.SetActive(false);
        }

    }

    public Spell GetNewSpell(SpellID spell)
    {
        return  Instantiate(_spellDict[spell]);
    }

    public Spell GetSpell(SpellID spell)
    {
        return _spellDict[spell];
    }

}

public enum SpellID{
    Fireball,
    Steam,
    PhysicalAttack,
    IceShard
}
