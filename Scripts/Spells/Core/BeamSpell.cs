using UnityEngine;
using System.Collections;
using System;

public class BeamSpell : ElementalSpell
{
    private float _lastApplyTime;

    public float beamApplyDelay;
    public bool chargeOnApply = true;

    public virtual float BeamSpellApplyDelay
    {
        get { return beamApplyDelay; }
    }

    public override SpellType SpellType
    {
        get
        {
            return global::SpellType.Beam;
        }
    }

    public override void CollisionEvent(Collider other)
    {
        base.CollisionEvent(other);
        if (Time.time - _lastApplyTime > BeamSpellApplyDelay && other.gameObject.layer == LayerMask.NameToLayer("Entity"))
        {
            if (chargeOnApply)
            {
                if (!CastingEntity.CanCastSpell(this))
                {
                    DestroySpell();
                    return;
                }
                CastingEntity.SubtractSpellCost(this);
            }
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
