using UnityEngine;
using System.Collections;

public class BeamMotor : SpellEffect
{
    public float beamApplyDelay = 0.1f;
    public bool chargeOnApply = false;

    private Timer beamDelayTimer;
    public float speed = 2f;
    public float maxDistance = 50f;
    public bool triggerStopMovement = true;

    private float distance = 1;
    private LayerMask ignoreLayers;

    public Vector3 BeamDirection
    {
        get
        {
            Vector3 dir = (transform.forward).normalized;
        //    dir.y = 0;
            return dir;
        }
    }

    public Vector3 BeamLocation
    {
        get
        {
            return transform.position + (transform.forward * distance);
        }
    }

    protected override void Start()
    {
        base.Start();
        CanKeepBeamOpen();
        beamDelayTimer = new Timer(beamApplyDelay);

        ignoreLayers = ~((1 << LayerMask.NameToLayer("Spell")) | (1 << LayerMask.NameToLayer("Ignore Raycast")));
    }

    protected override void UpdateSpell()
    {
        base.UpdateSpell();
        if (!CanKeepBeamOpen())
            return;
        transform.parent.rotation = effectSetting.spell.SpellStartTransform.rotation;
        transform.parent.position = effectSetting.spell.SpellStartTransform.position;
        Debug.DrawRay(transform.position, transform.forward * distance, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(new Ray(transform.position, transform.forward), out hit, distance, ignoreLayers))
        {

            if (hit.collider.gameObject == effectSetting.spell.CastingEntity.gameObject)
                return;
            distance = hit.distance;
            if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Ground") && hit.collider.gameObject.layer != LayerMask.NameToLayer("Spell"))
            {
                effectSetting.TriggerCollision(new ColliderEventArgs(hit.point), hit.collider);
            }
        }
        else
            distance = Mathf.Min(distance + (speed * Time.deltaTime), maxDistance);
    }

    protected override void effectSetting_OnSpellDestroy(object sender, SpellEventargs e)
    {
        base.effectSetting_OnSpellDestroy(sender, e);
        enabled = false;
    }

    /// <summary>
    /// //////////////////
    /// </summary>

    public bool KeepBeamAlive
    {
        get
        {
            return effectSetting.spell.CastingEntity.KeepBeamAlive();
        }
    }

    protected override void effectSetting_OnSpellCollision(ColliderEventArgs args, Collider obj)
    {
        base.effectSetting_OnSpellCollision(args, obj);
        if (beamDelayTimer.CanTickAndReset() && obj.gameObject.layer == LayerMask.NameToLayer("Entity"))
        {
            if (chargeOnApply)
                // Subtract the spell cost when this spell is applied only
                effectSetting.spell.CastingEntity.SubtractSpellCost(effectSetting.spell);

            Entity e = obj.GetComponent<Entity>();
            if (e != null)
                effectSetting.TriggerApplySpell(e);
        }
    }


    private bool CanKeepBeamOpen()
    {
        // The spell cast delay can mess up with beam spells. If it can't tick this will return false in that case
        // the current beam will stop. Be careful when using cast delay with beams.
        if (!effectSetting.spell.CastingEntity.CanCastSpell(effectSetting.spell) || !KeepBeamAlive)
        {
            effectSetting.TriggerDestroy();
            return false;
        }
        return true;
    }

}
