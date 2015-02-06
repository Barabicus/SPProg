using UnityEngine;
using System.Collections;

public class Knockback : TimedUpdateableEffect
{
    public float radius = 5f;

    protected override void UpdateSpell()
    {
        base.UpdateSpell();
        Collider[] cols = Physics.OverlapSphere(effectSetting.transform.position, radius, 1 << LayerMask.NameToLayer("Entity"));
        foreach (Collider c in cols)
        {
            if (c.gameObject == effectSetting.spell.CastingEntity.gameObject)
                continue;
            c.GetComponent<Entity>().Knockdown(1000f, effectSetting.transform.position, radius);
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    

}
