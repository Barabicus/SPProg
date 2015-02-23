using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EntityHitText))]
[RequireComponent(typeof(Rigidbody))]
public abstract class Entity : MonoBehaviour
{
    #region Fields
    public float currentHP;
    public float maxHP;
    public float speed = 5f;
    [HideInInspector]
    public float spellCastDelay = 0.1f;
    public string EntityName = "NOTSET";
    public Transform castPoint;
    public EntityFlags entityFlags;
    public ElementalStats elementalResistance = new ElementalStats(1,1,1,1,1);
    public ElementalStats maxElementalCharge = new ElementalStats(1, 1, 1, 1, 1);
    public ElementalStats rechargeRate = new ElementalStats(1, 1, 1, 1, 1);

    private ElementalStats _currentElementalCharge;
    private Animator animator;
    private Vector3 _lastPosition;
    private float _currentSpeed;
    private EntityLivingState _livingState = EntityLivingState.Alive;
    private Dictionary<string, Spell> _attachedSpells = new Dictionary<string, Spell>();
    private Rigidbody rigidbody;
    private EntityMotionState _entityMotionState;
    private EntityStats _baseStats;
    private List<EntityStats> _statModifiers = new List<EntityStats>();
    private EntityStats _cachedStats;
    private Timer knockdownTime;

    protected NavMeshAgent navMeshAgent;
    protected BeamSpell beamSpell = null;
    protected Transform selectedTarget;
    /// <summary>
    /// The timer that controls whether or not enough time has passed to be able to recast a spell again. This takes in spellcastdelay and will
    /// allow a spellrecast once that time has passed.
    /// </summary>
    protected Timer spellCastTimer;



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

    public enum EntityMotionState
    {
        Static,
        Pathfinding,
        Rigidbody
    }

    #endregion

    #region Properties

    public EntityStats BaseStats
    {
        get { return _baseStats; }
        set { _baseStats = value; }
    }

    public float Speed
    {
        get
        {
            return _cachedStats.speed;
        }
    }

    public float MaxHP
    {
        get
        {
            return maxHP;
        }
    }

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
                    MotionState = EntityMotionState.Static;
                    if (entityKilled != null)
                        entityKilled(this, new EntityEventArgs(this));
                    EntityKilled();
                    break;
                case EntityLivingState.Alive:
                    animator.SetBool("Dead", false);
                    foreach (Collider c in GetComponents<Collider>())
                        c.enabled = true;
                    foreach (Transform t in transform)
                        foreach (Collider c in t.GetComponents<Collider>())
                            c.enabled = true;
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
            currentHP = Mathf.Clamp(value, 0f, MaxHP);
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
            _currentElementalCharge = new ElementalStats(Mathf.Min(value[Element.Fire], maxElementalCharge[Element.Fire]), Mathf.Min(value[Element.Water], maxElementalCharge[Element.Water]), Mathf.Min(value[Element.Air], maxElementalCharge[Element.Air]), Mathf.Min(value[Element.Earth], maxElementalCharge[Element.Earth]), Mathf.Min(value[Element.Kinetic], maxElementalCharge[Element.Kinetic]));
        }
    }

    public ElementalStats MaxElementalCharge
    {
        get
        {
            return maxElementalCharge;
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
        get { return elementalResistance; }
    }

    public EntityMotionState MotionState
    {
        get { return _entityMotionState; }
        set
        {
            switch (value)
            {
                case EntityMotionState.Static:
                    rigidbody.isKinematic = true;
                    navMeshAgent.enabled = false;
                    break;
                case EntityMotionState.Pathfinding:
                    rigidbody.isKinematic = true;
                    navMeshAgent.enabled = true;
                    UpdateStatComponents();
                    break;
                case EntityMotionState.Rigidbody:
                    rigidbody.isKinematic = false;
                    navMeshAgent.enabled = false;
                    break;
            }
            _entityMotionState = value;
        }
    }

    #endregion

    #region Initialization
    protected virtual void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
    }

    protected virtual void Start()
    {
        _baseStats = new EntityStats(speed, maxHP);
        AddStatModifier(_baseStats);
        //    rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        MotionState = EntityMotionState.Pathfinding;
        _lastPosition = transform.position;
        _currentElementalCharge = MaxElementalCharge;
        //  maxElementalCharge = new ElementalStats(50, 50, 50);

        // Ensure HP is properly clamped
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP); ;
        // Ensure Elemental charge is properly clamped
        CurrentElementalCharge = CurrentElementalCharge;

        knockdownTime = new Timer(2f);
    }

    #endregion


    #region Updates
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
        switch (MotionState)
        {
            case EntityMotionState.Pathfinding:
                NavMeshUpdate();
                break;
            case EntityMotionState.Rigidbody:
                RigidBodyUpdate();
                break;
            case EntityMotionState.Static:
                StaticUpdate();
                break;
        }
    }

    /// <summary>
    /// Called while the entity is Living
    /// </summary>
    protected virtual void LivingUpdate()
    {
        CurrentElementalCharge += rechargeRate * Time.deltaTime;
    }
    /// <summary>
    /// Called while the entity is Dead
    /// </summary>
    protected virtual void DeadUpdate() { }

    /// <summary>
    /// Called while the rigid body is controlling entity motion
    /// </summary>
    protected virtual void RigidBodyUpdate()
    {
        if (knockdownTime.CanTickAndReset())
            MotionState = EntityMotionState.Pathfinding;
    }
    /// <summary>
    /// Called while the navmesh is controlling entity motion
    /// </summary>
    protected virtual void NavMeshUpdate()
    {
        if (navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
            LivingState = EntityLivingState.Dead;
    }
    /// <summary>
    /// Called while the entity motion is not being controlled by anything
    /// </summary>
    protected virtual void StaticUpdate() { }

    #endregion

    #region State And Value Changes

    /// <summary>
    /// Adjusts the health of the entity ensuring that the amount passed in is a whole number
    /// </summary>
    /// <param name="amount"></param>
    public void AdjustHealthByAmount(float amount)
    {
        amount = Mathf.Floor(amount);
        CurrentHP += amount;
    }

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

    public void AddStatModifier(EntityStats stat)
    {
        _statModifiers.Add(stat);
        _cachedStats += stat;
        AdjustHealthByAmount(stat.health);
        UpdateStatComponents();
    }

    public void RemoveStatModifier(EntityStats stat)
    {
        _statModifiers.Remove(stat);
        _cachedStats = _cachedStats.Difference(stat);
        UpdateStatComponents();
    }

    private void UpdateStatComponents()
    {
        navMeshAgent.speed = Mathf.Max(_cachedStats.speed, 1f);
    }

    public void Knockdown(float force, Vector3 explodePosition, float radius)
    {
        knockdownTime.Reset();
        MotionState = EntityMotionState.Rigidbody;
        rigidbody.AddExplosionForce(force, explodePosition, radius);
    }

    #endregion

    #region Entity Events

    protected virtual void Die()
    {
        LivingState = EntityLivingState.Dead;

    }

    /// <summary>
    /// Called when this entity has been killed
    /// </summary>
    protected virtual void EntityKilled()
    {

    }

    /// <summary>
    /// The logic the entity should use to keep the beam alive should be implemented here
    /// </summary>
    /// <returns></returns>
    protected abstract bool KeepBeamAlive();

    /// <summary>
    /// Called when a spell applys itself to an entity. The spell event agrs include details
    /// about the spell effect occuring on this entity. This includes the casting enity and other
    /// details which detail the origins of the spell.
    /// </summary>
    public virtual void SpellCastBy(SpellEventargs args) { }

    #endregion

    #region Spells


    public bool CastSpell(Spell spell)
    {
        Spell ta;
        return CastSpell(spell.SpellID, out ta);
    }

    public bool CastSpell(string spellID)
    {
        Spell ta;
        return CastSpell(spellID, out ta);
    }

    public bool CastSpell(string spell, out Spell castSpell)
    {
        return CastSpell(SpellList.Instance.GetSpell(spell), out castSpell);
    }

    public bool CastSpell(Spell spell, out Spell castSpell)
    {
        if (!CanCastSpell(spell) || IsBeamActive || (spell.SpellType == SpellType.Attached && !CanAttachSpell(spell)))
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
                ((BeamSpell)sp).KeepBeamAlive = () => { return KeepBeamAlive(); };
                beamSpell = sp as BeamSpell;
                beamSpell.OnSpellDestroy += (o, e) => { beamSpell = null; };
                break;
            case SpellType.Attached:
                AttachSpell(sp);
                break;
        }

        // Take spell cost
        SubtractSpellCost(sp);
        castSpell = sp;
        spellCastTimer = new Timer(spell.SpellCastDelay);
        return true;
    }

    public void SubtractSpellCost(Spell spell)
    {
        CurrentElementalCharge -= spell.ElementalCost;
    }

    public bool CanCastSpell(string spell)
    {
        return CanCastSpell(SpellList.Instance.GetSpell(spell));
    }

    public bool CanCastSpell(Spell spell)
    {
        if (!spellCastTimer.CanTick)
            return false;
        foreach (Element e in Enum.GetValues(typeof(Element)))
        {
            if (CurrentElementalCharge[e] < spell.ElementalCost[e])
                return false;
        }
        return true;
    }

    public bool CanAttachSpell(Spell spell)
    {
        return CanAttachSpell(spell.SpellID);
    }

    public bool CanAttachSpell(string spellID)
    {
        return !_attachedSpells.ContainsKey(spellID);
    }

    public void AttachSpell(Spell spell)
    {
        if (!CanAttachSpell(spell))
            Debug.LogError("Spell already contained and should not be attached!");
        _attachedSpells.Add(spell.SpellID, spell);
        spell.transform.parent = transform;
        spell.transform.position = transform.position;
        spell.OnSpellDestroy += RemoveAttachedSpell;
    }

    private void RemoveAttachedSpell(object sender, SpellEventargs e)
    {
        _attachedSpells.Remove(e.spell.SpellID);
    }

    #endregion

    #region Helper Methods

    public void EntityLookAt(Vector3 lookPosition)
    {
        lookPosition = new Vector3(lookPosition.x, transform.position.y, lookPosition.z);
        transform.LookAt(lookPosition);
    }

    #endregion


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
