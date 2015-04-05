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
            effectSetting.TriggerDestroy();

        if (waterIsZero && effectSetting.spell.CastingEntity.CurrentElementalCharge.water == 0)
            effectSetting.TriggerDestroy();

        if (airIsZero && effectSetting.spell.CastingEntity.CurrentElementalCharge.air == 0)
            effectSetting.TriggerDestroy();

        if (earthIsZero && effectSetting.spell.CastingEntity.CurrentElementalCharge.earth == 0)
            effectSetting.TriggerDestroy();

    }

}
