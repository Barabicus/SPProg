using UnityEngine;
using System.Collections;

public class BeamMotor : SpellEffect
{

    public float speed = 2f;
    public float maxDistance = 50f;
    public bool triggerStopMovement = true;

    private float distance = 1;

    private Vector3 entityOffset;

    private Vector3 EntityOffset
    {
        get
        {
            return effectSetting.spell.CastingEntity.transform.position - entityOffset;
        }
    }

    private Vector3 Direction
    {
        get
        {
            Vector3 dir = (effectSetting.spell.CastingEntity.transform.forward).normalized;
            dir.y = 0;
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
        entityOffset = effectSetting.spell.CastingEntity.transform.position - effectSetting.spell.SpellStartPosition;       
    }

    protected override void UpdateSpell()
    {
        base.UpdateSpell();
            transform.parent.rotation = effectSetting.spell.CastingEntity.transform.rotation;
            transform.parent.position = effectSetting.spell.SpellStartTransform.position;
            Debug.DrawRay(transform.position, transform.forward * distance, Color.red);
            RaycastHit hit;
            if (Physics.Raycast(new Ray(transform.position, transform.forward), out hit, distance))
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
                distance = Mathf.Min(distance +  (speed * Time.deltaTime), maxDistance);
    }

    protected override void effectSetting_OnSpellDestroy(object sender, SpellEventargs e)
    {
        base.effectSetting_OnSpellDestroy(sender, e);
        enabled = false;
    }

}
