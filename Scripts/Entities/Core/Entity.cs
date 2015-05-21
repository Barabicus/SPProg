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
    private List<EntityStats> _statModifiers = new List<EntityStats>();
    private EntityStats _cachedStats;
    private Timer _knockdownTime;
    private AudioSource _audio;
    private Timer _audioPlayTimer;
    private Spell _activeBeam;
    private NavMeshAgent _navMeshAgent;
    private Timer _updateSpeedTimer;

    protected Transform SelectedTarget;
    protected Animator Animator;

    #endregion

    #region Events

    public event Action<Entity> EntityKilled;
    public event Action<float> EntityHealthChanged;
    /// <summary>
    /// This event fires when an elemental spell has been cast on the entity
    /// </summary>
    public event Action<Entity, Spell> ElementalSpellCastOnEntity;
    public event Action<Spell> EntityCastedSpell;
    /// <summary>
    /// Much like ElementalSpellCastOnEntity this event fires when an elemental spell has been cast on an entity but results in the death of the entity
    /// </summary>
    public event Action<Entity, Spell> ElementalDeathSpellCastOnEntity;

    #endregion

    #region Properties

    private List<string> SpellMarkers { get; set; }

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
        set
        {
            _speed = value;
        }
        get
        {
            return _cachedStats.speed;
        }
    }

    public float MaxHp
    {
        get
        {
            return _maxHp;
        }
    }

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

    public NavMeshAgent NavMeshAgent
    {
        get { return _navMeshAgent; }
    }

    public Transform CastPoint
    {
        get { return _castPoint; }
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
    }

    protected virtual void Start()
    {
        EntityHealthChanged += HealthChanged;
        _baseStats = new EntityStats(_speed);
        AddStatModifier(_baseStats);
        //    rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        MotionState = EntityMotionState.Pathfinding;
        _lastPosition = transform.position;
        _currentElementalCharge = MaxElementalCharge;
        //  maxElementalCharge = new ElementalStats(50, 50, 50);

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

    private void UpdateSpeed()
    {
        var moveAmount = transform.position - _lastPosition;
        _lastPosition = transform.position;
        CurrentSpeed = moveAmount.magnitude / Time.deltaTime;
        // Normalise speed
        //  CurrentSpeed /= 5f;
    }

    public void AddStatModifier(EntityStats stat)
    {
        _statModifiers.Add(stat);
        _cachedStats += stat;
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
        _navMeshAgent.speed = Mathf.Max(_cachedStats.speed, 1f);
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

    public bool CastSpell(Spell spell, out Spell castSpell, Transform spellTarget = null, Vector3? spellTargetPosition = null)
    {
        if (!CanCastSpell(spell) || IsBeamActive || (spell.SpellType == SpellType.Attached && !CanAttachSpell(spell)))
        {
            castSpell = null;
            return false;
        }

        if (spell.spellType == SpellType.Attached)
            spellTarget = transform;

        Spell sp = SpellList.Instance.GetNewSpell(spell);
        sp.CastSpell(this, CastPoint, null, spellTarget, spellTargetPosition);
        // sp.SetupSpellTransform(_castPoint);

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

        // Take spellID cost
        if (!SpellsIgnoreElementalCost)
            SubtractSpellCost(sp);
        castSpell = sp;
        SpellCastTimer = new Timer(spell.SpellCastDelay);
        if (EntityCastedSpell != null)
            EntityCastedSpell(sp);
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

    public bool CanAttachSpell(Spell spell)
    {
        return CanAttachSpell(spell.SpellID);
    }

    public bool CanAttachSpell(string spellId)
    {
        return !_attachedSpells.ContainsKey(spellId);
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
