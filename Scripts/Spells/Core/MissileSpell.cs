using UnityEngine;
using System.Collections;

public class MissileSpell : ElementalSpell {

    public override void CollisionEvent(Collider other)
    {
        base.CollisionEvent(other);
        if (other.gameObject != CastingEntity.gameObject && other.gameObject.layer == LayerMask.NameToLayer("Entity"))
        {
            ApplySpell(other.GetComponent<Entity>());
        }
    }


    public override SpellType SpellType
    {
        get { return global::SpellType.Missile; }
    }
}
