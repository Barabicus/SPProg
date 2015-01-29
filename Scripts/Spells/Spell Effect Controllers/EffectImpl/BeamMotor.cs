using UnityEngine;
using System.Collections;

public class BeamMotor : SpellEffect
{

    public float speed = 2f;
    public bool triggerStopMovement = true;

    private float moveMultiplier = 1;

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

    protected override void Start()
    {
        base.Start();
        entityOffset = effectSetting.spell.CastingEntity.transform.position - effectSetting.spell.SpellStartPosition;       
    }

    protected override void Update()
    {
        base.Update();
            transform.parent.rotation = effectSetting.spell.CastingEntity.transform.rotation;
            transform.parent.position = effectSetting.spell.SpellStartTransform.position;
            Debug.DrawRay(transform.position, transform.forward * moveMultiplier, Color.red);
            RaycastHit hit;
            if (Physics.Raycast(new Ray(transform.position, transform.forward), out hit, moveMultiplier))
            {
                if (hit.collider.gameObject == effectSetting.spell.CastingEntity.gameObject)
                    return;
                moveMultiplier = hit.distance;
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Entity"))
                {
                    effectSetting.TriggerCollision(hit.collider);
                }
            }
            else
                moveMultiplier += speed * Time.deltaTime;
    }

}
