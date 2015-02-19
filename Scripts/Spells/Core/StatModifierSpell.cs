using UnityEngine;
using System.Collections;

public class StatModifierSpell : Spell
{

    public float speedMod = 0f;
    public float hpMod = 0f;

    private EntityStats _incrementStat;
    private EntityStats _addedStat;
    private Entity targetedEntity;

    public override void Start()
    {
        base.Start();
        _incrementStat = new EntityStats(speedMod, hpMod);
    }

    public override void ApplySpell(Entity entity)
    {
        base.ApplySpell(entity);
        if (targetedEntity == null)
            targetedEntity = entity;

        if (targetedEntity != null && targetedEntity == entity)
        {
            entity.AddStatModifier(_incrementStat);
            _addedStat += _incrementStat;
        }
    }

    public override void DestroyEvent()
    {
        base.DestroyEvent();
        if (targetedEntity != null)
        {
            targetedEntity.RemoveStatModifier(_addedStat);
        }
    }

}
