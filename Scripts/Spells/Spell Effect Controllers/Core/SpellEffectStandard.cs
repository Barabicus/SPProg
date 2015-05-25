using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

/// <summary>
/// The standard spell effect allows us a standard of setting up and handling specific spell related effect events.
/// Inheriting from this class means we can easily listen for when a Event is triggered and deal with it appropriately. 
/// Using this we can generalise many different spell effects to behave the same but respond to different triggers.
/// </summary>
public abstract class SpellEffectStandard : SpellEffect
{

    public SpellEffectTriggerEvent triggerEvent;
    [Tooltip("If using a timed trigger event this is how long it will take to trigger the event")]
    public float timeTrigger = 1f;
    [FormerlySerializedAs("timeSingleShot")]
    public bool isSingleShot = false;

    private Timer timedEvent;

    private bool r_enabled;

    public enum SpellEffectTriggerEvent
    {
        Timed,
        Collision,
        SpellApply,
        SpellDestroy,
        EffectDestroy,
        Cast
    }

    public override void InitializeEffect(EffectSetting effectSetting)
    {
        base.InitializeEffect(effectSetting);
        r_enabled = enabled;
    }

    protected override void OnSpellStart()
    {
        base.OnSpellStart();
        enabled = r_enabled;
        timedEvent = new Timer(timeTrigger);
    }

    protected override void effectSetting_OnSpellCast()
    {
        base.effectSetting_OnSpellCast();
        if (triggerEvent == SpellEffectTriggerEvent.Cast)
            EventTriggered();
    }

    protected override void effectSetting_OnSpellCollision(ColliderEventArgs args, Collider obj)
    {
        base.effectSetting_OnSpellCollision(args, obj);
        if (triggerEvent == SpellEffectTriggerEvent.Collision)
            EventTriggered();
    }

    protected override void effectSetting_OnSpellDestroy()
    {
        base.effectSetting_OnSpellDestroy();
        if (triggerEvent == SpellEffectTriggerEvent.SpellDestroy)
            EventTriggered();
    }

    protected override void effectSetting_OnEffectDestroy()
    {
        base.effectSetting_OnEffectDestroy();
        if (triggerEvent == SpellEffectTriggerEvent.EffectDestroy)
            EventTriggered();
    }

    protected override void effectSetting_OnSpellApply(Entity entity)
    {
        base.effectSetting_OnSpellApply(entity);
        if (triggerEvent == SpellEffectTriggerEvent.SpellApply)
            EventTriggered();
    }

    protected override void UpdateSpell()
    {
        base.UpdateSpell();
        if (triggerEvent == SpellEffectTriggerEvent.Timed && timedEvent.CanTickAndReset())
        {
            if (isSingleShot)
                enabled = false;
            EventTriggered();
        }
    }

    private void EventTriggered()
    {
        DoEventTriggered();

    }

    protected virtual void DoEventTriggered()
    {

    }

}
