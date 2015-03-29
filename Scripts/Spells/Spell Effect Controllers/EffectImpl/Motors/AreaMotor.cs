using UnityEngine;
using System.Collections;

public class AreaMotor : TimedUpdateableSpellMotor
{
    public float radius = 5f;

    protected override void Start()
    {
        base.Start();
    }

    protected override void UpdateSpell()
    {
        base.UpdateSpell();
        CheckCast();
    }

    private void CheckCast()
    {
        Collider[] colls = Physics.OverlapSphere(effectSetting.transform.position, radius, 1 << 9);
        foreach (Collider c in colls)
        {
            if (c.gameObject != effectSetting.spell.CastingEntity.gameObject)
            {
                TryTriggerCollision(new ColliderEventArgs(), c);
                //effectSetting.TriggerCollision(new ColliderEventArgs(), c);
            }
        }
    }

    protected override void effectSetting_OnSpellCollision(ColliderEventArgs args, Collider obj)
    {
        base.effectSetting_OnSpellCollision(args, obj);
        if (obj.gameObject.layer == LayerMask.NameToLayer("Entity"))
        {
            Entity e = obj.gameObject.GetComponent<Entity>();
            if (e != null)
                effectSetting.TriggerApplySpell(e);
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.parent.position, radius);
    }



}
