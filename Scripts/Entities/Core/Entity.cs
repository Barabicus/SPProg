using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EntityHitText))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class Entity : MonoBehaviour
{
    #region Fields
    [SerializeField]
    private float currentHP;
    [SerializeField]
    private float maxHP;
    [SerializeField]
    private float speed = 5f;
    [HideInInspector]
    private float spellCastDelay = 0.1f;
    [SerializeField]
    private string entityName = "NOTSET";
    [SerializeField]
    private Transform castPoint;
    [SerializeField]
    private ParticleSystem hitParticlesPrefab;
    [SerializeField]
    private ParticleSystem deathParticlesPrefab;
    [SerializeField]
    private EntityFlags entityFlags;
    [SerializeField]
    private ElementalStats elementalResistance = ElementalStats.One;
    [SerializeField]
    private ElementalStats maxElementalCharge = ElementalStats.One;
    [SerializeField]
    private ElementalStats rechargeRate = ElementalStats.One;
    [SerializeField]
    private AudioClip deathAudio;

    private ElementalStats _currentElementalCharge;
    /// <summary>
    /// Recharge lock on each element based on time i.e fire = 1 is a lock of 1 second on fire recharge
    /// </summary>
    private ElementalStats _rechargeLock = ElementalStats.One;
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
    private AudioSource audio;
    private Timer _audioPlayTimer;
    private Spell activeBeam;

 //   private Timer spellCastTimer;

    protected NavMeshAgent navMeshAgent;
    protected Transform selectedTarget;
    protected Animator animator;


    #endregion

    #region Events

    public event Action<Entity> entityKilled;
    public event Action<float> entityHealthChanged;
    /// <summary>
    /// An Event with the spell that was just cast. Note this is an instance to the actual spell that was cast and exists in game.
    /// </summary>
    public event Action<Spell> spellCast;

    #endregion

    #region Properties

    public float CurrentSpeed
    {
        get { return _currentSpeed; }
        set { _currentSpeed = value; }
    }

    public string EntityName
    {
        get { return entityName; }
    }

    public EntityStats BaseStats
    {
        get { return _baseStats; }
        set { _baseStats = value; }
    }

    /// <summary>
    /// The timer that controls whether or not enough time has passed to be able to recast a spell again. This takes in spellcastdelay and will
    /// allow a spellrecast once that time has passed.
    /// </summary>
    public Timer SpellCastTimer
    {
        get;
        set;
    }
    public float Speed
    {
        set
        {
            speed = value;
        }
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

    /// <summary>
    /// Specified by the entity if it wants to keep the beam open
    /// </summary>
    public bool KeepBeamOpen
    {
        get;
        set;
    }

    /// <summary>
    /// Returns true if this beam should be kept open
    /// </summary>
    public bool CanOpenBeam
    {
        get
        {
            if (LivingState != EntityLivingState.Alive || !KeepBeamOpen)
                return false;
            else
                return true;
        }
    }

    public NavMeshAgent NavMeshAgent
    {
        get { return navMeshAgent; }
    }

    public Transform CastPoint
    {
        get { return castPoint; }
    }

    public ElementalStats RechargeRate
    {
        get
        {
            ElementalStats et = new ElementalStats(0, 0, 0, 0, 0);
            foreach (Element e in Enum.GetValues(typeof(Element)))
            {
                et[e] = _rechargeLock[e] == 1 ? 1 : 0;
            }
            return rechargeRate * et;
        }
    }

    /// <summary>
    /// Check to see if the beam is active. This prevents the beam being cast multiple time upon use
    /// </summary>
    public bool IsBeamActive
    {
        get { return activeBeam != null; }
    }
    public EntityLivingState LivingState
    {
        get { return _livingState; }
        set
        {
            switch (value)
            {
                case EntityLivingState.Dead:
                    MotionState = EntityMotionState.Static;
                    if (entityKilled != null)
                        entityKilled(this);
                    break;
                case EntityLivingState.Alive:
                    MotionState = EntityMotionState.Pathfinding;
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

    public ParticleSystem HitParticles
    {
        get;
        set;
    }

    public ParticleSystem DeathParticles
    {
        get;
        set;
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
        set
        {
            maxElementalCharge = value;
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
        audio = GetComponent<AudioSource>();
    }

    protected virtual void Start()
    {
        entityHealthChanged += HealthChanged;
        _baseStats = new EntityStats(speed, 0);
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
        _audioPlayTimer = new Timer(0.25f);
        HitParticles = Instantiate(hitParticlesPrefab);
        DeathParticles = Instantiate(deathParticlesPrefab);
        HitParticles.transform.parent = transform;
        DeathParticles.transform.parent = transform;
        HitParticles.transform.localPosition = new Vector3(0, 1, 0);
        DeathParticles.transform.localPosition = Vector3.zero;
    }

    #endregion


    #region Updates
    protected virtual void Update()
    {
        switch (LivingState)
        {
            case EntityLivingState.Alive:
                UpdateSpeed();
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
        UpdateRechargeLock();
        CurrentElementalCharge += RechargeRate * Time.deltaTime;
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
        //  if (navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
        //       LivingState = EntityLivingState.Dead;
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

    private void UpdateSpeed()
    {
        var moveAmount = transform.position - _lastPosition;
        _lastPosition = transform.position;
        CurrentSpeed = moveAmount.magnitude / Time.deltaTime;
        // Normalise speed
        CurrentSpeed /= 5f;
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

    protected virtual void HealthChanged(float amount)
    {
        if (amount < 0 && HitParticles != null && UnityEngine.Random.Range(0, 2) == 0)
            HitParticles.Emit(10);

    }

    private void Die()
    {
        LivingState = EntityLivingState.Dead;
        if (DeathParticles != null)
            DeathParticles.Emit(UnityEngine.Random.Range(2, 5));
        PlaySound(deathAudio);
    }

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
            case SpellType.Beam:
                activeBeam = sp;
                sp.OnSpellDestroy += (o, e) => { activeBeam = null; };
                break;
            case SpellType.Attached:
                AttachSpell(sp);
                break;
        }

        // Take spell cost
        SubtractSpellCost(sp);
        castSpell = sp;
        SpellCastTimer = new Timer(spell.SpellCastDelay);
        if (spellCast != null)
            spellCast(sp);
        PlaySound(castSpell.castAudio);
        return true;
    }
    public void SubtractSpellCost(Spell spell)
    {
        CurrentElementalCharge -= spell.ElementalCost;
        SetRechargeLock(spell.ElementalCost);
    }

    public void SubtractElementCost(ElementalStats element)
    {
        CurrentElementalCharge -= element;
        SetRechargeLock(element);
    }

    public bool CanCastSpell(string spell)
    {
        return CanCastSpell(SpellList.Instance.GetSpell(spell));
    }

    public bool CanCastSpell(Spell spell)
    {
        if (!SpellCastTimer.CanTick)
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

    public void UpdateRechargeLock()
    {
        foreach (Element e in Enum.GetValues(typeof(Element)))
        {
            _rechargeLock[e] = Mathf.MoveTowards(_rechargeLock[e], 1f, Time.deltaTime);
        }
    }

    /// <summary>
    /// Set the recharge lock to lock a spell if a passed in stat is greater than 0
    /// </summary>
    /// <param name="stats"></param>
    public void SetRechargeLock(ElementalStats stats)
    {
        foreach (Element e in Enum.GetValues(typeof(Element)))
        {
            // Setting recharge lock to 0 will cause the element of that type not to be updated
            _rechargeLock[e] = stats[e] > 0 ? 0 : 1;
        }
    }

    public void EntityLookAt(Vector3 lookPosition)
    {
        lookPosition = new Vector3(lookPosition.x, transform.position.y, lookPosition.z);
        transform.LookAt(lookPosition);
    }

    private void PlaySound(AudioClip audioClip)
    {
        if (audioClip != null && _audioPlayTimer.CanTickAndReset())
            audio.PlayOneShot(audioClip);
    }

    private void EnableColliders()
    {
        foreach (Collider c in GetComponents<Collider>())
            c.enabled = true;
        foreach (Transform t in transform)
            foreach (Collider c in t.GetComponents<Collider>())
                c.enabled = true;
    }

    private void DisableColliders()
    {
        foreach (Collider c in GetComponents<Collider>())
            c.enabled = false;
        foreach (Transform t in transform)
            foreach (Collider c in t.GetComponents<Collider>())
                c.enabled = false;
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
