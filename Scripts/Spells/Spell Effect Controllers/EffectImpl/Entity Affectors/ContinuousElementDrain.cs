using UnityEngine;
using System.Collections;

public class ContinuousElementDrain : SpellEffect
{
    public float tickTime;
    public ElementalStats drainOnTick = ElementalStats.Zero;

    private Timer timer;
    protected override void Start()
    {
        base.Start();
        timer = new Timer(tickTime);        
    }

    protected override void UpdateSpell()
    {
        base.UpdateSpell();
        if (timer.CanTickAndReset())
        {
           // effectSetting.spell.CastingEntity.CurrentElementalCharge -= drainOnTick;
            effectSetting.spell.CastingEntity.SubtractElementCost(drainOnTick);
        }
    }

}
