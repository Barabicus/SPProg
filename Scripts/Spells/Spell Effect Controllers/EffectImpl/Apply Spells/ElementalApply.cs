using UnityEngine;
using System.Collections;


/// <summary>
/// Applies the elemental stats to the target entity's health
/// </summary>
public class ElementalApply : SpellEffect
{
    public ElementalStats elementalPower = ElementalStats.Zero;

    protected override void effectSetting_OnSpellApply(Entity entity)
    {
        base.effectSetting_OnSpellApply(entity);

        if (entity.LivingState != Entity.EntityLivingState.Alive)
            return;

        // Apply the spells elemental properties
        foreach (Element e in System.Enum.GetValues(typeof(Element)))
        {
            entity.AdjustHealthByAmount(elementalPower[e] * -entity.ElementalModifier[e]);
        }

    }

}
