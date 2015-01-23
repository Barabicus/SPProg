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
    public string EntityName = "NOTSET";
    public float fire, water;


    public EntityFlags entityFlags;
    
    private ElementalStats _elementalModifier;
    private Animator animator;

    private Vector3 _lastPosition;
    private float _currentSpeed;

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
        animator = GetComponent<Animator>();

        _lastPosition = transform.position;

        // Ensure HP is properly clamped
        CurrentHP = CurrentHP;
        _elementalModifier = new ElementalStats(fire, water);
    }

    public Spell CastSpell(SpellID spell, Vector3 startPosition)
    {
        Spell sp = SpellList.Instance.GetSpell(spell);
        sp.CastSpell(this, startPosition);
        return sp;
    }

    protected virtual void Update()
    {
        UpdateSpeed();
        UpdateAnimation();
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
