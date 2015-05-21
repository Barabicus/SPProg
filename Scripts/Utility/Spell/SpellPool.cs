using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellPool : MonoBehaviour
{

    public int preloadAmount = 10;

    public static SpellPool Instance { get; set; }

    private Dictionary<string, Queue<Spell>> _spellPool;
    private Dictionary<string, Spell> _spellPrefabs;

    private void Awake()
    {
        Instance = this;
        _spellPool = new Dictionary<string, Queue<Spell>>();
        _spellPrefabs = new Dictionary<string, Spell>();
        _spellPrefabs = Resources.LoadAll<SpellListInfo>("Utility")[0].SpellDictionary;
        PreloadPool();

    }

    private void PreloadPool()
    {
        foreach (var spell in _spellPrefabs.Values)
        {
            _spellPool.Add(spell.SpellID, new Queue<Spell>());
            for (int i = 0; i < preloadAmount; i++)
            {
                PoolSpell(CreateNewSpell(spell.SpellID));
            }
        }
    }

    public Spell GetSpellFromPool(string spellID)
    {
        var queue = _spellPool[spellID];
        Spell sp = null;
        if (queue.Count > 0)
        {
            sp = queue.Dequeue();
            sp.enabled = true;
        }
        else
        {
            PoolSpell(CreateNewSpell(spellID));
            return GetSpellFromPool(spellID);
        }
        return sp;
    }

    private Spell CreateNewSpell(string spellID)
    {
        var sp = Instantiate(_spellPrefabs[spellID]);
        sp.InitializeSpell();
        sp.gameObject.SetActive(false);
        return sp;
    }

    public void PoolSpell(Spell spell)
    {
        spell.gameObject.SetActive(false);
        _spellPool[spell.spellID].Enqueue(spell);
    }
}
