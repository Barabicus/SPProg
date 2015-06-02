using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker.Actions;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class Entity : MonoBehaviour
{
    #region Fields
    [SerializeField]
    private float _currentHp = 0;
    [SerializeField]
    private float _maxHp = 0;
    [SerializeField]
    private float _speed = 5f;
    [SerializeField]
    private string _entityName = "NOTSET";
    [SerializeField]
    private string _entityID;
    [SerializeField]
    private bool _isInvincible = false;
    [SerializeField]
    private bool loadHealthBar = true;
    [SerializeField]
    private bool loadSpeechBubble = false;
    [SerializeField]
    private bool loadMinimapIcon = true;
    [SerializeField]
    private Transform _castPoint = null;
    [SerializeField]
    private ParticleSystem _hitParticlesPrefab = null;
    [SerializeField]
    private ParticleSystem _deathParticlesPrefab = null;
    [SerializeField]
    private FactionFlags _factionFlags = FactionFlags.Two;
    [SerializeField]
    private bool _spellsIgnoreElementalCost = false;
    [SerializeField]
    private ElementalStats _spellElementalModifier = ElementalStats.One;
    [SerializeField]
    private ElementalStats _elementalResistance = ElementalStats.One;
    [SerializeField]
    private ElementalStats _maxElementalCharge = ElementalStats.One;
    [SerializeField]
    private ElementalStats _rechargeRate = ElementalStats.One;
    [SerializeField]
    private AudioClip _deathAudio = null;

    private ElementalStats _currentElementalCharge = ElementalStats.Zero;
    /// <summary>
    /// Recharge lock on each element based on time i.e fire = 1 is a lock of 1 second on fire recharge
    /// </summary>
    private ElementalStats _rechargeLock = ElementalStats.One;
    private Vector3 _lastPosition = Vector3.zero;
    private float _currentSpeed = 0f;
    private EntityLivingState _livingState = EntityLivingState.Alive;
    private Dictionary<string, Spell> _attachedSpells = new Dictionary<string, Spell>();
    private Rigidbody _rigidbody = null;
    private EntityMotionState _entityMotionState = EntityMotionState.Static;
    private EntityStats _baseStats;
    private EntityStats _cachedStats;
    private Timer _knockdownTime;
    private AudioSource _audio;
    private Timer _audioPlayTimer;
    private Spell _activeBeam;
    private NavMeshAgent _navMeshAgent;
    private Timer _updateSpeedTimer;
    private Dictionary<string, List<EntityStats>> _entityStatModifiers;

    protected Transform SelectedTarget;
    protected Animator Animator;

    private const string _baseStatsID = "entity_base_stats";

    #endregion

    #region Events

    public event Action<Entity> EntityKilled;
    public event Action<float> EntityHealthChanged;
    /// <summary>
    /// This event fires when an elemental spell has been cast on the entity
    /// </summary>
    public event Action<Entity, Spell> ElementalSpellCastOnEntity;
    /// <summary>
    /// Much like ElementalSpellCastOnEntity this event fires when an elemental spell has been cast on an entity but results in the death of the entity
    /// </summary>
    public event Action<Entity, Spell> ElementalDeathSpellCastOnEntity;

    #endregion

    #region Properties

    private List<string> SpellMarkers { get; set; }
    /// <summary>
    /// The modifiers such as Debuffs or Buffs that are modifying the Entity's stats. It is associated with a spell and uses
    /// the spell ID to look up the spell. Each spell may apply many different stats depending if the spell is stackable or not. 
    /// When the spell is removed it iterates through all the Modifiers associated with the spell and removes them.
    /// </summary>
    public Dictionary<string, List<EntityStats>> EntityStatModifiers { get { return _entityStatModifiers; } }

    public bool SpellsIgnoreElementalCost
    {
        get { return _spellsIgnoreElementalCost; }
        set { _spellsIgnoreElementalCost = value; }
    }

    public FactionFlags EntityFactionFlags
    {
        get { return _factionFlags; }
        set { _factionFlags = value; }
    }

    public ElementalStats SpellElementalModifier
    {
        get { return _spellElementalModifier; }
        set { _spellElementalModifier = value; }
    }

    public float CurrentSpeed
    {
        get { return _currentSpeed; }
        set { _currentSpeed = value; }
    }

    public string EntityName
    {
        get { return _entityName; }
    }

    public string EntityID
    {
        get { return _entityID; }
    }

    public EntityStats BaseStats
    {
        get { return _baseStats; }
        set { _baseStats = value; }
    }

    public EntityStats CachedStats
    {
        get { return _cachedStats; }
        private set { _cachedStats = value; }
    }

    /// <summary>
    /// The timer that controls whether or not enough time has passed to be able to recast a spellID again. This takes in spellcastdelay and will
    /// allow a spellrecast once that time has passed.
    /// </summary>
    public Timer SpellCastTimer
    {
        get;
        set;
    }
    public float Speed
    {
        get
        {
            return CachedStats.Speed;
        }
    }
    /// <summary>
    /// This is the maximum HP of the Enity.
    /// </summary>
    public float MaxHp
    {
        get
        {
            return _maxHp;
        }
    }
    /// <summary>
    /// A normalised value of the Entity's health. This only returns a value
    /// and cannot be set. If you need to set the health do so through the CurrentHP property.
    /// </summary>
    public float CurrentHealthNormalised
    {
        get { return CurrentHp / MaxHp; }
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
    /// <summary>
    /// The nav mesh agent associated with the Entity
    /// </summary>
    public NavMeshAgent NavMeshAgent
    {
        get { return _navMeshAgent; }
    }
    /// <summary>
    /// This is where the Entity will cast spells froms
    /// </summary>
    public Transform CastPoint
    {
        get { return _castPoint; }
    }
    /// <summary>
    /// This is the recharge rate of the Entity's elements. It will return the value
    /// that the Entity should increment it's current charge, taking into account if the 
    /// respective element is locked or not.
    /// </summary>
    public ElementalStats RechargeRate
    {
        get
        {
            ElementalStats et = new ElementalStats(0, 0, 0, 0, 0);
            foreach (Element e in Enum.GetValues(typeof(Element)))
            {
                et[e] = _rechargeLock[e] == 1 ? 1 : 0;
            }
            return _rechargeRate * et;
        }
    }

    /// <summary>
    /// Check to see if the beam is active. This prevents the beam being cast multiple time upon use
    /// </summary>
    public bool IsBeamActive
    {
        get { return _activeBeam != null; }
    }
    /// <summary>
    /// This is the current living state of the Entity. Setting this will also properly
    /// handle setting up the state of the Entity to match the living state.
    /// </summary>
    public EntityLivingState LivingState
    {
        get { return _livingState; }
        set
        {
            switch (value)
            {
                case EntityLivingState.Dead:
                    MotionState = EntityMotionState.Static;
                    if (EntityKilled != null)
                        EntityKilled(this);
                    break;
                case EntityLivingState.Alive:
                    MotionState = EntityMotionState.Pathfinding;
                    break;
            }
            _livingState = value;
        }
    }
    /// <summary>
    /// This returns the current HP of the Enity. It will also properly handle setting the HP based on the
    /// Entity's state.
    /// </summary>
    public float CurrentHp
    {
        get { return _currentHp; }
        set
        {
            if (_isInvincible)
                return;
            float oldHealth = _currentHp;
            _currentHp = Mathf.Clamp(value, 0f, MaxHp);
            if (EntityHealthChanged != null)
                EntityHealthChanged(_currentHp - oldHealth);
            if (_currentHp == 0f && LivingState == EntityLivingState.Alive)
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
            _currentElementalCharge = new ElementalStats(Mathf.Clamp(value[Element.Fire], 0, _maxElementalCharge[Element.Fire]), Mathf.Clamp(value[Element.Water], 0, _maxElementalCharge[Element.Water]), Mathf.Clamp(value[Element.Air], 0, _maxElementalCharge[Element.Air]), Mathf.Clamp(value[Element.Earth], 0, _maxElementalCharge[Element.Earth]), Mathf.Clamp(value[Element.Kinetic], 0, _maxElementalCharge[Element.Kinetic]));
        }
    }

    public ElementalStats MaxElementalCharge
    {
        get
        {
            return _maxElementalCharge;
        }
        set
        {
            _maxElementalCharge = value;
        }
    }

    public ElementalStats ElementalModifier
    {
        get { return _elementalResistance; }
    }

    public EntityMotionState MotionState
    {
        get { return _entityMotionState; }
        set
        {
            switch (value)
            {
                case EntityMotionState.Static:
                    _rigidbody.isKinematic = true;
                    _navMeshAgent.enabled = false;
                    break;
                case EntityMotionState.Pathfinding:
                    _rigidbody.isKinematic = true;
                    _navMeshAgent.enabled = true;
                    UpdateStatComponents();
                    break;
                case EntityMotionState.Rigidbody:
                    _rigidbody.isKinematic = false;
                    _navMeshAgent.enabled = false;
                    break;
            }
            _entityMotionState = value;
        }
    }

    public EntitySpeechBubble SpeechBubble { get; set; }

    public EntityHealthBar HealthBar { get; set; }

    #region Animation Properties

    public HumanoidAnimatorController HumanController
    {
        get;
        set;
    }
    public bool IsCastingTriggered { get; set; }

    #endregion

    #endregion

    #region Initialization
    protected virtual void Awake()
    {
        SpellMarkers = new List<string>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _audio = GetComponent<AudioSource>();
        _updateSpeedTimer = new Timer(UnityEngine.Random.Range(0.15f, 0.35f));
        _entityStatModifiers = new Dictionary<string, List<EntityStats>>();
        SpellCastTimer = new Timer(0f);
    }

    protected virtual void Start()
    {
        EntityHealthChanged += HealthChanged;
        BaseStats = new EntityStats(_speed);
        ApplyStatModifier(_baseStatsID, BaseStats);

        MotionState = EntityMotionState.Pathfinding;
        _lastPosition = transform.position;
        _currentElementalCharge = MaxElementalCharge;

        // Ensure HP is properly clamped
        _currentHp = Mathf.Clamp(_currentHp, 0f, _maxHp); ;
        // Ensure Elemental charge is properly clamped
        CurrentElementalCharge = CurrentElementalCharge;

        _knockdownTime = new Timer(2f);
        _audioPlayTimer = new Timer(0.25f);
        if (_hitParticlesPrefab != null)
        {
            HitParticles = Instantiate(_hitParticlesPrefab);
            HitParticles.transform.parent = transform;
            HitParticles.transform.localPosition = new Vector3(0, 1, 0);
        }
        if (_deathParticlesPrefab != null)
        {
            DeathParticles = Instantiate(_deathParticlesPrefab);
            DeathParticles.transform.parent = transform;
            DeathParticles.transform.localPosition = new Vector3(0, 0.25f, 0);
        }

        // Create Speech Bubble
        if (loadSpeechBubble)
        {
            SpeechBubble =
                Instantiate(GameMainReferences.Instance.GameConfigInfo.EntitySpeechBubblePrefab);
            SpeechBubble.Entity = this;
            //  SpeechBubble.transform.SetParent(transform);
        }

        if (loadHealthBar)
        {
            HealthBar = Instantiate(GameMainReferences.Instance.GameConfigInfo.EntityHealthBarPrefab);
            HealthBar.transform.SetParent(transform);
        }

        if (loadMinimapIcon)
        {
            var miniMapIcon = Instantiate(GameMainReferences.Instance.GameConfigInfo.EntityMinimapIcon);
            miniMapIcon.Entity = this;
        }

    }

    #endregion

    #region Updates
    protected virtual void Update()
    {
        switch (LivingState)
        {
            case EntityLivingState.Alive:
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
        //   if (_updateSpeedTimer.CanTickAndReset())
        UpdateSpeed();

        if (!SpellsIgnoreElementalCost)
        {
            UpdateRechargeLock();
            CurrentElementalCharge += RechargeRate * Time.deltaTime;
        }
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
        if (_knockdownTime.CanTickAndReset())
            MotionState = EntityMotionState.Pathfinding;
    }
    /// <summary>
    /// Called while the navmesh is controlling entity motion
    /// </summary>
    protected virtual void NavMeshUpdate()
    {

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
        CurrentHp += amount;
    }
    /// <summary>
    /// Updates the current speed the Entity is moving at. This is useful for animation.
    /// </summary>
    private void UpdateSpeed()
    {
        var moveAmount = transform.position - _lastPosition;
        _lastPosition = transform.position;
        CurrentSpeed = moveAmount.magnitude / Time.deltaTime;
        // Normalise speed
        //  CurrentSpeed /= 5f;
    }

    /// <summary>
    /// Modifies the EntityStats to add the amount passed in. It will automatically update the state of the Entity to reflect
    /// the new stats.
    /// </summary>
    /// <param name="stat"></param>
    public void ApplyStatModifier(string id, EntityStats stat)
    {
        if (!_entityStatModifiers.ContainsKey(id))
        {
            _entityStatModifiers.Add(id, new List<EntityStats>());
        }
        _entityStatModifiers[id].Add(stat);
        // Apply the stat to the cached stats
        CachedStats += stat;
        // Update the stats of the Entity to reflect the new stats
        UpdateStatComponents();
    }
    /// <summary>
    /// Removes all EnityStats applied using the specified ID. It will automatically update the state of the Entity to reflect
    /// the new stats.
    /// </summary>
    /// <param name="stat"></param>
    public void RemoveAllStatModifiers(string id)
    {
        if (!_entityStatModifiers.ContainsKey(id))
        {
            Debug.LogError("Trying to remove modifier with invalid ID");
        }
        else
        {
            // Iterate through all EntityStats and remove them from the cached stats
            foreach (EntityStats s in _entityStatModifiers[id])
            {
                CachedStats = CachedStats.Difference(s);
            }
            // Clear the list
            _entityStatModifiers[id].Clear();
        }
        // Update the stats of the Entity to reflect the new stats
        UpdateStatComponents();
    }
    /// <summary>
    /// Updates the Entity to match the cached stats
    /// </summary>
    private void UpdateStatComponents()
    {
        _navMeshAgent.speed = Mathf.Max(CachedStats.Speed, 1f);
    }

    public void Knockdown(float force, Vector3 explodePosition, float radius)
    {
        _knockdownTime.Reset();
        MotionState = EntityMotionState.Rigidbody;
        _rigidbody.AddExplosionForce(force, explodePosition, radius);
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
            DeathParticles.Emit(UnityEngine.Random.Range(5, 25));
        PlaySound(_deathAudio);
        EntityManager.Instance.TriggerEntityKilled(this);
    }

    private void ElementalDeathSpell(Spell spell)
    {
        switch (spell.SpellDeathMarker)
        {
            case SpellDeathMarker.None:
                break;
            case SpellDeathMarker.Explode:
                break;
            case SpellDeathMarker.Freeze:
                break;
            case SpellDeathMarker.Burn:
                break;
        }
    }

    #endregion



    #region Spells

    public bool HasSpellMarker(string spellID)
    {
        return SpellMarkers.Contains(spellID);
    }

    public void AddSpellMarker(string spellID)
    {
        SpellMarkers.Add(spellID);
    }

    public void RemoveSpellMarker(string spellID)
    {
        SpellMarkers.Remove(spellID);
    }

    public void ApplyElementalSpell(ElementalApply elementalSpell)
    {
        bool wasLiving = LivingState == EntityLivingState.Alive;

        foreach (Element e in Enum.GetValues(typeof(Element)))
        {
            AdjustHealthByAmount((elementalSpell.ElementalPower[e] * elementalSpell.effectSetting.spell.CastingEntity.SpellElementalModifier[e]) * -ElementalModifier[e]);
        }

        if (ElementalSpellCastOnEntity != null)
            ElementalSpellCastOnEntity(elementalSpell.effectSetting.spell.CastingEntity, elementalSpell.effectSetting.spell);

        if (wasLiving && LivingState == EntityLivingState.Dead)
        {
            if (ElementalDeathSpellCastOnEntity != null)
                ElementalDeathSpellCastOnEntity(elementalSpell.effectSetting.spell.CastingEntity,
                    elementalSpell.effectSetting.spell);

            ElementalDeathSpell(elementalSpell.effectSetting.spell);
        }

    }

    public bool CastSpell(Spell spell)
    {
        Spell ta;
        return CastSpell(spell.SpellID, out ta);
    }

    public bool CastSpell(string spellId)
    {
        Spell ta;
        return CastSpell(spellId, out ta);
    }

    public bool CastSpell(string spell, out Spell castSpell)
    {
        return CastSpell(SpellList.Instance.GetSpell(spell), out castSpell);
    }
    /// <summary>
    /// This creates a new spell that the Entity cast itself. This mean the spell will recognise this Enity
    /// as the spell caster. It ensures that the Entity can only cast the spell if possible and returns if it cannot.
    /// It also handles the casting of different types of spells. For example if an attached spell is cast it will 
    /// attach it appropriately 
    /// </summary>
    /// <param name="spell"></param>
    /// <param name="castSpell"></param>
    /// <param name="spellTarget"></param>
    /// <param name="spellTargetPosition"></param>
    /// <returns></returns>
    public bool CastSpell(Spell spell, out Spell castSpell, Transform spellTarget = null, Vector3? spellTargetPosition = null)
    {
        if (!CanCastSpell(spell) || IsBeamActive)
        {
            castSpell = null;
            return false;
        }

        // If we are attaching a spell ensure the spell target is set to this transform.
        if (spell.spellType == SpellType.Attached)
            spellTarget = transform;

        Spell sp = null;
        if (spell.SpellType != SpellType.Attached || (spell.SpellType == SpellType.Attached && !HasAttachedSpell(spell)))
            sp = CreateNewSpellFromEntity(spell, spellTarget, spellTargetPosition);
        else
            RefreshAttachedSpell(spell);

        // Take spellID cost
        if (!SpellsIgnoreElementalCost)
            SubtractSpellCost(spell);
        castSpell = sp;
        SpellCastTimer.TickTime = spell.SpellCastDelay;
        SpellCastTimer.Reset();
        PlaySound(spell.castAudio);
        return true;
    }

    private Spell CreateNewSpellFromEntity(Spell spell, Transform spellTarget, Vector3? spellTargetPosition)
    {
        Spell sp = SpellList.Instance.GetNewSpell(spell);
        sp.CastSpell(this, CastPoint, null, spellTarget, spellTargetPosition);

        switch (sp.SpellType)
        {
            case SpellType.Beam:
                _activeBeam = sp;
                sp.OnSpellDestroy += (s) => { _activeBeam = null; };
                break;
            case SpellType.Attached:
                AttachSpell(sp);
                break;
        }
        return sp;
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
        if (!SpellCastTimer.CanTick || LivingState != EntityLivingState.Alive)
            return false;
        return HasElementalChargeToCast(spell);
    }

    public bool HasElementalChargeToCast(Spell spell)
    {
        if (SpellsIgnoreElementalCost)
            return true;

        foreach (Element e in Enum.GetValues(typeof(Element)))
        {
            if (CurrentElementalCharge[e] < spell.ElementalCost[e])
                return false;
        }
        return true;
    }

    public bool HasAttachedSpell(Spell spell)
    {
        return HasAttachedSpell(spell.SpellID);
    }

    public bool HasAttachedSpell(string spellId)
    {
        return _attachedSpells.ContainsKey(spellId);
    }

    public void AttachSpell(Spell spell)
    {
        if (HasAttachedSpell(spell))
            Debug.LogError("Spell already contained and should not be attached!");
        _attachedSpells.Add(spell.SpellID, spell);
        spell.transform.parent = transform;
        spell.transform.position = transform.position;
        spell.OnSpellDestroy += RemoveAttachedSpell;
    }

    public void RefreshAttachedSpell(Spell spell)
    {
        if (!HasAttachedSpell(spell))
        {
            Debug.LogError("Attached spell does not exist!");
            return;
        }
        _attachedSpells[spell.SpellID].SpellDestroyTimer.Reset();

    }

    private void RemoveAttachedSpell(Spell spell)
    {
        _attachedSpells.Remove(spell.SpellID);
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
    /// Set the recharge lock to lock a spellID if a passed in stat is greater than 0
    /// </summary>
    /// <param name="stats"></param>
    public void SetRechargeLock(ElementalStats stats)
    {
        foreach (Element e in Enum.GetValues(typeof(Element)))
        {
            // Setting recharge lock to 0 will cause the element of that type not to be updated
            _rechargeLock[e] = stats[e] > 0 ? 0 : _rechargeLock[e];
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
            _audio.PlayOneShot(audioClip);
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

    #region Entity Relations

    public bool IsEnemy(Entity other)
    {
        if ((other.EntityFactionFlags & EntityFactionFlags) == 0)
            return true;
        else
            return false;
    }

    #endregion
}

public class EntityEventArgs : EventArgs
{
    public Entity Entity;

    public EntityEventArgs(Entity entity)
    {
        this.Entity = entity;
    }
}

[Flags]
public enum FactionFlags
{
    One = 1 << 0,
    Two = 1 << 1,
    Three = 1 << 2,
    Four = 1 << 3
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
