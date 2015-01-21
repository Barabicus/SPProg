using UnityEngine;
using System.Collections;

public abstract class ElementalSpell : Spell
{

    public override abstract float SpellLiveTime
    {
        get;
    }

    public override abstract SpellID SpellID
    {
        get;
    }

    /// <summary>
    /// The Elemental properties of this spell
    /// </summary>
    public abstract ElementalStats ElementalPower
    {
        get;
    }

    public override void ApplySpell(Entity entity)
    {
        base.ApplySpell(entity);

        // Apply the spells elemental properties
        foreach (Element e in System.Enum.GetValues(typeof(Element)))
        {
            entity.CurrentHP += ElementalPower.GetElementalStat(e) * -entity.ElementalModifier.GetElementalStat(e);
        }
    }
}
