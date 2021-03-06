﻿using UnityEngine;
using System.Collections;

public class AddStatModifier : SpellEffect
{

    public float speedMod = 0f;
    [Tooltip("Used to Directly affect the entity's health regardless of elemental resistence. This can be used for heals")]
    public float hpMod = 0f;


    /// <summary>
    /// How much each apply will increment the entity stat
    /// </summary>
    private EntityStats _incrementStat;
    /// <summary>
    /// The total stats added when the spell is finished it will revert the applied stats
    /// </summary>
    private EntityStats _addedStat;
    private Entity targetedEntity;

    protected override void Start()
    {
        base.Start();
        _incrementStat = new EntityStats(speedMod, hpMod);
    }

    protected override void effectSetting_OnSpellApply(Entity entity)
    {
        base.effectSetting_OnSpellApply(entity);
        if (targetedEntity == null)
            targetedEntity = entity;

        if (targetedEntity != null && targetedEntity == entity)
        {
            entity.AddStatModifier(_incrementStat);
            _addedStat += _incrementStat;
        }
    }

    protected override void effectSetting_OnSpellDestroy(object sender, SpellEventargs e)
    {
        base.effectSetting_OnSpellDestroy(sender, e);
        if (targetedEntity != null)
        {
            targetedEntity.RemoveStatModifier(_addedStat);
        }
    }

}
