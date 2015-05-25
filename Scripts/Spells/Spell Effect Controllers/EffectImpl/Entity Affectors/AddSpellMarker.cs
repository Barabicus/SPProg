using UnityEngine;
using System.Collections;

public class AddSpellMarker : SpellEffectStandard
{
 
    private Entity _targetEntity;
    private bool _triggerFired;
    /// <summary>
    /// Add a marker on the next frame, this prevents other scripts from messing up.
    /// </summary>
    private bool _addMarker;

    private bool _hasAddedMarker;

    protected override void OnSpellStart()
    {
        base.OnSpellStart();
        _addMarker = false;
        _hasAddedMarker = false;
        _triggerFired = false;
        _targetEntity = null;
    }

    protected override void effectSetting_OnSpellApply(Entity entity)
    {
        base.effectSetting_OnSpellApply(entity);
        if (_triggerFired)
            return;
        if (!entity.HasSpellMarker(SpellMarker))
        {
            _targetEntity = entity;
            _addMarker = true;
        }
    }

    protected override void DoEventTriggered()
    {
        base.DoEventTriggered();
        // Prevent the spell marker being removed more than once by this
        _triggerFired = true;
        if (_hasAddedMarker)
        {
            _targetEntity.RemoveSpellMarker(SpellMarker);
        }
    }

    private void LateUpdate()
    {
        if (_addMarker && !_triggerFired && !_hasAddedMarker)
        {
            _targetEntity.AddSpellMarker(SpellMarker);
            _hasAddedMarker = true;
            _addMarker = false;
        }
    }

    public void Reset()
    {
        triggerEvent = SpellEffectTriggerEvent.EffectDestroy;
    }

}
