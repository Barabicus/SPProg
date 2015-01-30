using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
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

    public event EventHandler<EntityEventArgs> entityKilled;

    private ElementalStats _elementalResistance;
    private Animator animator;
    private Vector3 _lastPosition;
    private float _currentSpeed;
    private EntityLivingState _livingState = EntityLivingState.Alive;

    protected NavMeshAgent navMeshAgent;
    protected BeamSpell beamSpell = null;
    protected Transform selectedTarget;


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
                        Destroy(c);
                    foreach (Transform t in transform)
                        foreach (Collider c in t.GetComponents<Collider>())
                            Destroy(c);
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
            currentHP = Mathf.Clamp(value, 0f, maxHP);
            if (currentHP == 0)
                Die();
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

        // Ensure HP is properly clamped
        CurrentHP = CurrentHP;
        _elementalResistance = new ElementalStats(fire, water, kinetic);
    }


    public Spell CastSpell(SpellID spell)
    {
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

        return sp;
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
