using UnityEngine;
using System.Collections;
using System;

public class BeamSpell : ElementalSpell
{
    private float _lastApplyTime;

    public float beamApplyDelay;
    public bool chargeOnApply = true;

    private Timer beamDelayTimer;

    public virtual float BeamSpellApplyDelay
    {
        get { return beamApplyDelay; }
    }

    public override void CollisionEvent(Collider other)
    {
        if (beamDelayTimer.CanTickAndReset() && other.gameObject.layer == LayerMask.NameToLayer("Entity"))
        {
            if (chargeOnApply)
            {
                if (!CanKeepBeamOpen())
                    return;
                // Subtract the spell cost when this spell is applied only
                CastingEntity.SubtractSpellCost(this);
            }

            ApplySpell(other.GetComponent<Entity>());
            if (chargeOnApply)
                ChargeBeamCost();

        }
    }

    public Func<bool> KeepBeamAlive;

    private bool CanKeepBeamOpen()
    {
        // The spell cast delay can mess up with beam spells. If it can't tick this will return false in that case
        // the current beam will stop. Be careful when using cast delay with beams.
        if (!CastingEntity.CanCastSpell(this))
        {
            DestroySpell();
            return false;
        }
        return true;
    }

    private void ChargeBeamCost()
    {
        CastingEntity.SubtractSpellCost(this);
        _lastApplyTime = Time.time;
    }

    public override void Start()
    {
        base.Start();
        beamDelayTimer = new Timer(beamApplyDelay);
        _lastApplyTime = Time.time;
        if (KeepBeamAlive == null)
            DestroySpell();
    }

    public override void Update()
    {
        base.Update();
        if (!KeepBeamAlive())
            DestroySpell();
        if (!chargeOnApply)
            ChargeBeamCost();

        CanKeepBeamOpen();
    }
}
