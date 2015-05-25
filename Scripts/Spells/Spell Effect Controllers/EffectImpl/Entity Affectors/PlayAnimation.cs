using UnityEngine;
using System.Collections;

public class PlayAnimation : SpellEffectStandard
{
    [SerializeField]
    private HumanoidEntityAnimation _humanAnimation;

    public HumanoidEntityAnimation HumanAnimation
    {
        get { return _humanAnimation; }
    }

    protected override void DoEventTriggered()
    {
        base.DoEventTriggered();
        Entity e = effectSetting.spell.CastingEntity;
        if (e.HumanController != null)
            e.HumanController.PlayAnimation(HumanAnimation);
    }

    public void Reset()
    {
        triggerEvent = SpellEffectTriggerEvent.Cast;
    }

}


