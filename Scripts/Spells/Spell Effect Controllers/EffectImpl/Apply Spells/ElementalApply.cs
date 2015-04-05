using System;
using UnityEngine;
using System.Collections;


/// <summary>
/// Applies the elemental stats to the target entity's health
/// </summary>
public class ElementalApply : SpellEffect
{
    [SerializeField]
    private ElementalStats elementalPower = ElementalStats.Zero;
    [SerializeField]
    private ApplyTo _applyTo = ApplyTo.TargetEntity;

    public enum ApplyTo
    {
        TargetEntity,
        Caster
    }

    public ElementalStats ElementalPower
    {
        get { return elementalPower; }
        set { elementalPower = value; }
    }

    protected override void effectSetting_OnSpellApply(Entity entity)
    {
        base.effectSetting_OnSpellApply(entity);

        Entity ent = null;
        switch (_applyTo)
        {
            case ApplyTo.TargetEntity:
                ent = entity;
                break;
            case ApplyTo.Caster:
                ent = effectSetting.spell.CastingEntity;
                break;
        }

        if (ent.LivingState != EntityLivingState.Alive)
            return;

        // Apply the spells elemental properties
        ent.ApplyElementalSpell(this);

    }

}
