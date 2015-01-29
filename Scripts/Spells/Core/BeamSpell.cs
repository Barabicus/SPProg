using UnityEngine;
using System.Collections;
using System;

public abstract class BeamSpell : ElementalSpell
{
    private float _lastApplyTime;

    public override abstract float SpellLiveTime
    {
        get;
    }

    public override abstract SpellID SpellID
    {
        get;
    }

    public override abstract ElementalStats ElementalPower
    {
        get;
    }

    public virtual float BeamSpellApplyDelay
    {
        get { return 0.05f; }
    }

    public override SpellType SpellType
    {
        get
        {
            return global::SpellType.Beam;
        }
    }

    public override abstract float SpellCastDelay
    {
        get;
    }

    public override void CollisionEvent(Collider other)
    {
        base.CollisionEvent(other);
        if (Time.time - _lastApplyTime > BeamSpellApplyDelay && other.gameObject.layer == LayerMask.NameToLayer("Entity"))
        {
            ApplySpell(other.GetComponent<Entity>());
            _lastApplyTime = Time.time;
        }
    }

    public Func<bool> KeepBeamAlive;

    public override void Start()
    {
        base.Start();
        _lastApplyTime = Time.time;
        if (KeepBeamAlive == null)
            DestroySpell();
    }

    public override void Update()
    {
        base.Update();
        if (!KeepBeamAlive())
            DestroySpell();

    }
}
