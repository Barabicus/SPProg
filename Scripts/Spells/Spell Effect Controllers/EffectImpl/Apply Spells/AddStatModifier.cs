using UnityEngine;
using System.Collections;
/// <summary>
/// Handles the adding and removing of stat modifiers to the Enity. The applied stats will remain as long as the spell persists.
/// </summary>
public class AddStatModifier : SpellEffect
{

    [SerializeField]
    /// <summary>
    /// How much each apply will increment the entity stat
    /// </summary>
    private EntityStats _statModifier;
    /// <summary>
    /// The total stats added when the spell is finished it will revert the applied stats
    /// </summary>
    private Entity targetedEntity;

    public EntityStats StatModifier
    {
        get { return _statModifier; }
        set { _statModifier = value; }
    }

    protected override void OnSpellStart()
    {
        base.OnSpellStart();
    }

    protected override void effectSetting_OnSpellApply(Entity entity)
    {
        base.effectSetting_OnSpellApply(entity);
        if (targetedEntity == null)
            targetedEntity = entity;

        if (targetedEntity != null && targetedEntity == entity)
        {
            entity.ApplyStatModifier(effectSetting.spell.SpellID, _statModifier);
        }
    }

    protected override void effectSetting_OnSpellDestroy()
    {
        base.effectSetting_OnSpellDestroy();
        if (targetedEntity != null)
        {
            targetedEntity.RemoveAllStatModifiers(effectSetting.spell.SpellID);
        }
    }

}
