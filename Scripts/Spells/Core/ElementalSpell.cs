using UnityEngine;
using System.Collections;

public abstract class ElementalSpell : Spell
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
            entity.CurrentHP += ElementalPower.GetElementalStat(e) * -entity.ElementalModifier.GetElementalStat(e);
        }
    }

}
