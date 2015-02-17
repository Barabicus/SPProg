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
                Entity e = c.gameObject.GetComponent<Entity>();

             //   if ((!collideWithSameFlag && (e.entityFlags & effectSetting.spell.CastingEntity.entityFlags) != 0) || collideWithSameFlag)
                    effectSetting.TriggerCollision(new ColliderEventArgs(), c);
            }
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.parent.position, radius);
    }



}
