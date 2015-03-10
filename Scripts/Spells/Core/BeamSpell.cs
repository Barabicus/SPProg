using UnityEngine;
using System.Collections;
using System;

public class BeamSpell : ElementalSpell
{
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
            if (!CanKeepBeamOpen())
                return;

            if (chargeOnApply)
                // Subtract the spell cost when this spell is applied only
                CastingEntity.SubtractSpellCost(this);

            ApplySpell(other.GetComponent<Entity>());

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

    public override void Start()
    {
        base.Start();
        beamDelayTimer = new Timer(beamApplyDelay);
        if (KeepBeamAlive == null)
            DestroySpell();
    }

    public override void Update()
    {
        base.Update();
        if (!KeepBeamAlive())
            DestroySpell();

        CanKeepBeamOpen();
    }
}
