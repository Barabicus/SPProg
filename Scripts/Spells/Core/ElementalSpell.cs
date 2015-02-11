using UnityEngine;
using System.Collections;

public class ElementalSpell : Spell
{

    public float fire, water, kinetic;

    /// <summary>
    /// The Elemental properties of this spell
    /// </summary>
    public virtual ElementalStats ElementalPower
    {
        get { return new ElementalStats(fire, water, kinetic); }
    }

    public override void ApplySpell(Entity entity)
    {
        base.ApplySpell(entity);

        if (entity.LivingState != Entity.EntityLivingState.Alive)
            return;

        // Apply the spells elemental properties
        foreach (Element e in System.Enum.GetValues(typeof(Element)))
        {
            entity.CurrentHP += ElementalPower[e] * -entity.ElementalModifier[e];
        }
    }

    public override void CollisionEvent(Collider other)
    {
        base.CollisionEvent(other);
        if (other.gameObject != CastingEntity.gameObject && other.gameObject.layer == LayerMask.NameToLayer("Entity"))
        {
            ApplySpell(other.GetComponent<Entity>());
        }
    }
}
