using UnityEngine;
using System.Collections;

public abstract class MissileSpell : ElementalSpell {


    public override abstract float SpellLiveTime
    {
        get;
    }


    public override abstract SpellType SpellType
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

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != CastingEntity.gameObject && other.gameObject.layer == LayerMask.NameToLayer("Entity"))
        {
            SpellCollidedWithEntity(other.GetComponent<Entity>());
            ApplySpell(other.GetComponent<Entity>());
        }
        if (other.gameObject != CastingEntity.gameObject && other.gameObject.layer != LayerMask.NameToLayer("Spell"))
        {
            SpellCollided(other);
            TriggerCollisionEvent();
        }
    }

    /// <summary>
    /// Called when the spell collides with anything
    /// </summary>
    /// <param name="other"></param>
    protected virtual void SpellCollided(Collider other)
    {
        DestroySpell();
    }

    /// <summary>
    /// Called when the spell collides with an entity. Note SpellCollided will still be called.
    /// </summary>
    /// <param name="entity"></param>
    protected virtual void SpellCollidedWithEntity(Entity entity) { }

}
