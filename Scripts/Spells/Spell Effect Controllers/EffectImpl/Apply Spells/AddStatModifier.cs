using UnityEngine;
using System.Collections;

public class AddStatModifier : SpellEffect
{

    public float speedMod = 0f;

    /// <summary>
    /// How much each apply will increment the entity stat
    /// </summary>
    private EntityStats _incrementStat;
    /// <summary>
    /// The total stats added when the spell is finished it will revert the applied stats
    /// </summary>
    private EntityStats _addedStat;
    private Entity targetedEntity;

    protected override void OnSpellStart()
    {
        base.OnSpellStart();
        _incrementStat = new EntityStats(speedMod);
        _addedStat = default(EntityStats);
        targetedEntity = null;
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

    protected override void effectSetting_OnSpellDestroy()
    {
        base.effectSetting_OnSpellDestroy();
        if (targetedEntity != null)
        {
            targetedEntity.RemoveStatModifier(_addedStat);
        }
    }

}
