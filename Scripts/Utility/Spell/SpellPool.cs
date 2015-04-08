using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellPool : MonoBehaviour
{

    public static SpellPool Instance { get; set; }

    private Dictionary<string, Queue<Spell>> _spellPool;

    private void Awake()
    {
        Instance = this;
        _spellPool = new Dictionary<string, Queue<Spell>>();
    }

    public Spell GetSpellFromPool(string spellID)
    {
        var list = _spellPool[spellID];
        Spell sp = null;
        if (list.Count > 0)
        {
            sp = list.Dequeue();
            sp.enabled = true;
        }
        return sp;
    }

    public void InitSpellPool(Spell spell)
    {
        _spellPool.Add(spell.spellID, new Queue<Spell>());
    }

    public void PoolSpell(Spell spell)
    {
        _spellPool[spell.spellID].Enqueue(spell);
        spell.gameObject.SetActive(false);
    }
}
