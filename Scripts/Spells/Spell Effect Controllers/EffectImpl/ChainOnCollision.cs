using UnityEngine;
using System.Collections;

public class ChainOnCollision : SpellEffect {

    public float radius = 5f;

    protected override void effectSetting_OnSpellCollision(ColliderEventArgs args, Collider obj)
    {
        base.effectSetting_OnSpellCollision(args, obj);
        Collider[] colliders = Physics.OverlapSphere(obj.transform.position, radius, 1 << LayerMask.NameToLayer("Entity"));
        foreach (Collider c in colliders)
        {
            Spell sp = SpellList.Instance.GetNewSpell(effectSetting.spell);
            sp.CastSpell(effectSetting.spell.CastingEntity, obj.transform);
            sp.SpellTargetPosition = c.transform.position;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }



}
