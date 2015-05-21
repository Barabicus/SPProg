using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SpellList : MonoBehaviour
{

    public static SpellList Instance;

    private Dictionary<string, Spell> _spellDict;
    private SpellPool _spellPool;

    public void Awake()
    {
        Instance = this;
        _spellDict = Resources.LoadAll<SpellListInfo>("Utility")[0].SpellDictionary;
    }

    public void Start()
    {
        _spellPool = SpellPool.Instance;
    }

    public Spell[] Spells
    {
        get { return SpellDictionary.Values.ToArray(); }
    }

    public Dictionary<string, Spell> SpellDictionary
    {
        get { return _spellDict;}
    } 

    public Spell GetNewSpell(string spell)
    {
        Spell sp = _spellPool.GetSpellFromPool(spell);
        if (sp == null)
        {
            sp = Instantiate(_spellDict[spell]);
            sp.InitializeSpell();
        }
        return sp;
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