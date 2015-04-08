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

    public enum TriggerEvent
    {
        Timed,
        Collision,
        SpellApply,
        SpellDestroy,
        EffectDestroy,
        Cast
    }

    protected override void Start()
    {
        base.Start();
        timedEvent = new Timer(timeTrigger);
    }

    protected override void effectSetting_OnSpellCast()
    {
        base.effectSetting_OnSpellCast();
        if(triggerEvent == TriggerEvent.Cast)
            EventTriggered();
    }

    protected override void effectSetting_OnSpellCollision(ColliderEventArgs args, Collider obj)
    {
        base.effectSetting_OnSpellCollision(args, obj);
        if (triggerEvent == TriggerEvent.Collision)
            EventTriggered();
    }

    protected override void effectSetting_OnSpellDestroy(object sender, SpellEventargs e)
    {
        base.effectSetting_OnSpellDestroy(sender, e);
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
        if(triggerEvent == TriggerEvent.SpellApply)
            EventTriggered();
    }

    protected override void UpdateSpell()
    {
        base.UpdateSpell();
        if (triggerEvent == TriggerEvent.Timed && timedEvent.CanTickAndReset())
        {
            EventTriggered();
            if (isSingleShot)
                enabled = false;
        }
    }

    private void EventTriggered()
    {
        DoEventTriggered();
        if(isSingleShot)
            Destroy(this);
    }

    protected virtual void DoEventTriggered()
    {

    }

}
