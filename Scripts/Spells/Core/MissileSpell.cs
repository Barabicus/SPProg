using UnityEngine;
using System.Collections;

public abstract class MissileSpell : ElementalSpell {


    public override abstract float SpellLiveTime
    {
        get;
    }


    public override abstract SpellID SpellID
    {
        get;
    }

    public override abstract ElementalStats ElementalPower
    {
        get;
    }

    public override abstract float SpellCastDelay
    {
        get;
    }

    public override void CollisionEvent(Collider other)
    {
        base.CollisionEvent(other);
        if (other.gameObject != CastingEntity.gameObject && other.gameObject.layer == LayerMask.NameToLayer("Entity"))
        {
            ApplySpell(other.GetComponent<Entity>());
        }
        if (other.gameObject != CastingEntity.gameObject && other.gameObject.layer != LayerMask.NameToLayer("Spell"))
        {
            DestroySpell();
        }
    }


    public override SpellType SpellType
    {
        get { return global::SpellType.Missile; }
    }
}
