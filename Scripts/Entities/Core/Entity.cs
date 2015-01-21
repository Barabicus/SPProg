using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(NavMeshAgent))]
public class Entity : MonoBehaviour
{
    #region Fields
    public float currentHP;
    public float maxHP;
    public string EntityName = "NOTSET";
    public float fire, water;


    public EntityFlags entityFlags;
    
    private ElementalStats _elementalModifier;
    protected NavMeshAgent navMeshAgent;

    #endregion

    #region Properties

    public float CurrentHP
    {
        get { return currentHP; }
        set
        {
            currentHP = Mathf.Clamp(value, 0f, maxHP);
            if (currentHP == 0)
                Die();
        }
    }

    public ElementalStats ElementalModifier
    {
        get { return _elementalModifier; }
    }

    #endregion
    protected virtual void Awake() { }

    protected virtual void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        // Ensure HP is properly clamped
        CurrentHP = CurrentHP;
        _elementalModifier = new ElementalStats(fire, water);
    }

    public Spell CastSpell(SpellID spell)
    {
        Spell sp = SpellList.Instance.GetSpell(spell);
        sp.CastSpell(this);
        return sp;
    }

    protected virtual void Update() { }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

}

[Flags]
public enum EntityFlags
{
    One,
    Two,
    Three,
    Four
}
