using UnityEngine;
using System.Collections;

public class DestroySpellOnCondition : SpellEffect
{
    public bool fireIsZero;
    public bool waterIsZero;
    public bool airIsZero;
    public bool earthIsZero;

    protected override void UpdateSpell()
    {
        base.UpdateSpell();
        if (fireIsZero && effectSetting.spell.CastingEntity.CurrentElementalCharge.fire == 0)
            effectSetting.TriggerDestroySpell();

        if (waterIsZero && effectSetting.spell.CastingEntity.CurrentElementalCharge.water == 0)
            effectSetting.TriggerDestroySpell();

        if (airIsZero && effectSetting.spell.CastingEntity.CurrentElementalCharge.air == 0)
            effectSetting.TriggerDestroySpell();

        if (earthIsZero && effectSetting.spell.CastingEntity.CurrentElementalCharge.earth == 0)
            effectSetting.TriggerDestroySpell();

    }

}
