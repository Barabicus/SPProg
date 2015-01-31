using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EntityHitText))]
public class Entity : MonoBehaviour
{
    #region Fields
    public float currentHP;
    public float maxHP;
    public float spellCastDelay = 0.1f;
    public string EntityName = "NOTSET";
    public float fire = 1, water = 1, kinetic = 1;
    public Transform castPoint;
    public EntityFlags entityFlags;


    private ElementalStats _elementalResistance;
    private ElementalStats _currentElementalCharge;
    private ElementalStats _maxElementalCharge;
    private Animator animator;
    private Vector3 _lastPosition;
    private float _currentSpeed;
    private EntityLivingState _livingState = EntityLivingState.Alive;

    protected NavMeshAgent navMeshAgent;
    protected BeamSpell beamSpell = null;
    protected Transform selectedTarget;


    #endregion

    #region Events

    public event EventHandler<EntityEventArgs> entityKilled;
    public event Action<float> entityHealthChanged;

    #endregion 

    #region States

    public enum EntityLivingState
    {
        Alive,
        Dead
    }

    #endregion

    #region Properties

    public EntityLivingState LivingState
    {
        get { return _livingState; }
        set
        {
            switch (value)
            {
                case EntityLivingState.Dead:
                    animator.SetBool("Dead", true);
                    foreach (Collider c in GetComponents<Collider>())
                        c.enabled = false;
                    foreach (Transform t in transform)
                        foreach (Collider c in t.GetComponents<Collider>())
                            c.enabled = false;
                    navMeshAgent.enabled = false;
                    if (entityKilled != null)
                        entityKilled(this, new EntityEventArgs(this));
                    break;
                case EntityLivingState.Alive:
                    animator.SetBool("Dead", false);
                    navMeshAgent.enabled = true;
                    break;
            }
            _livingState = value;
        }
    }

    public float CurrentHP
    {
        get { return currentHP; }
        set
        {
            float oldHealth = currentHP;
            currentHP = Mathf.Clamp(value, 0f, maxHP);
            if (entityHealthChanged != null)
                entityHealthChanged(currentHP - oldHealth);
            if (currentHP == 0)
                Die();
        }
    }

    public ElementalStats CurrentElementalCharge
    {
        get
        {
            return _currentElementalCharge;
        }
        set
        {
            _currentElementalCharge = new ElementalStats(Mathf.Min(value[Element.Fire], _maxElementalCharge[Element.Fire]), Mathf.Min(value[Element.Water], _maxElementalCharge[Element.Water]), Mathf.Min(value[Element.Kinetic], _maxElementalCharge[Element.Kinetic]));
        }
    }

    public ElementalStats MaxElementalCharge
    {
        get
        {
            return _maxElementalCharge;
        }
    }

    public bool IsBeamActive
    {
        get
        {
            return beamSpell != null;
        }
    }

    public ElementalStats ElementalModifier
    {
        get { return _elementalResistance; }
    }

    #endregion
    protected virtual void Awake() { }

    protected virtual void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    
        _lastPosition = transform.position;
        _currentElementalCharge = new ElementalStats(50, 50, 50);
        _maxElementalCharge = new ElementalStats(50, 50, 50);

        // Ensure HP is properly clamped
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP); ;
        // Ensure Elemental charge is properly clamped
        CurrentElementalCharge = CurrentElementalCharge;
        _elementalResistance = new ElementalStats(fire, water, kinetic);
    }

    public bool CastSpell(Spell spell, out Spell castSpell)
    {
        return CastSpell(spell.spellID, out castSpell);
    }

    public bool CastSpell(string spell, out Spell castSpell)
    {
        if (!CanCastSpell(spell) || IsBeamActive)
        {
            castSpell = null;
            return false;
        }

        Spell sp = SpellList.Instance.GetNewSpell(spell);
        sp.CastSpell(this, castPoint);

        spellCastDelay = sp.SpellCastDelay;

        switch (sp.SpellType)
        {
            // Check if spell type is beam and do beam logic
            case SpellType.Beam:
                ((BeamSpell)sp).KeepBeamAlive = () => { return Input.GetMouseButton(1); };
                beamSpell = sp as BeamSpell;
                beamSpell.OnSpellDestroy += (o, e) => { beamSpell = null; };
                break;
        }
        castSpell = sp;
        return true;
    }

    public bool CanCastSpell(string spell)
    {
        return CanCastSpell(SpellList.Instance.GetSpell(spell));
    }

    public bool CanCastSpell(Spell spell)
    {
        foreach (Element e in Enum.GetValues(typeof(Element)))
        {
            if (CurrentElementalCharge[e] < spell.ElementalCost[e])
                return false;
        }
        return true;
    }

    protected virtual void Update()
    {
        switch (LivingState)
        {
            case EntityLivingState.Alive:
                UpdateSpeed();
                UpdateAnimation();
                LivingUpdate();
                break;
            case EntityLivingState.Dead:
                DeadUpdate();
                break;
        }
    }

    protected virtual void LivingUpdate() { }
    protected virtual void DeadUpdate() { }

    private void UpdateAnimation()
    {
        animator.SetFloat("Speed", _currentSpeed);
    }

    private void UpdateSpeed()
    {
        var moveAmount = transform.position - _lastPosition;
        _lastPosition = transform.position;
        _currentSpeed = moveAmount.magnitude / Time.deltaTime;
        // Normalise speed
        _currentSpeed /= navMeshAgent.speed;

    }

    protected virtual void Die()
    {
        //  Destroy(gameObject);
        LivingState = EntityLivingState.Dead;

    }

    /// <summary>
    /// Called when a spell applys itself to an entity. The spell event agrs include details
    /// about the spell effect occuring on this entity
    /// </summary>
    public virtual void SpellCastBy(SpellEventargs args) { }

}

public class EntityEventArgs : EventArgs
{
    public Entity entity;

    public EntityEventArgs(Entity entity)
    {
        this.entity = entity;
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
