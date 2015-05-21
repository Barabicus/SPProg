using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

public class SpellEffectStandard : SpellEffect
{

    public TriggerEvent triggerEvent;
    [Tooltip("If using a timed trigger event this is how long it will take to trigger the event")]
    public float timeTrigger = 1f;
    [FormerlySerializedAs("timeSingleShot")]
    public bool isSingleShot = false;

    private Timer timedEvent;

    private bool r_enabled;

    public enum TriggerEvent
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
        if (triggerEvent == TriggerEvent.Cast)
            EventTriggered();
    }

    protected override void effectSetting_OnSpellCollision(ColliderEventArgs args, Collider obj)
    {
        base.effectSetting_OnSpellCollision(args, obj);
        if (triggerEvent == TriggerEvent.Collision)
            EventTriggered();
    }

    protected override void effectSetting_OnSpellDestroy()
    {
        base.effectSetting_OnSpellDestroy();
        if (triggerEvent == TriggerEvent.SpellDestroy)
            EventTriggered();
    }

    protected override void effectSetting_OnEffectDestroy()
    {
        base.effectSetting_OnEffectDestroy();
        if (triggerEvent == TriggerEvent.EffectDestroy)
            EventTriggered();
    }

    protected override void effectSetting_OnSpellApply(Entity entity)
    {
        base.effectSetting_OnSpellApply(entity);
        if (triggerEvent == TriggerEvent.SpellApply)
            EventTriggered();
    }

    protected override void UpdateSpell()
    {
        base.UpdateSpell();
        if (triggerEvent == TriggerEvent.Timed && timedEvent.CanTickAndReset())
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
