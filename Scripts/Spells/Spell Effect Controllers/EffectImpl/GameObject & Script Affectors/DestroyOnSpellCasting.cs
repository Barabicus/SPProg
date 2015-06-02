using UnityEngine;
using System.Collections;

public class DestroyOnSpellCasting : SpellEffect
{

    protected override void UpdateSpell()
    {
        base.UpdateSpell();
        if (!effectSetting.spell.CastingEntity.IsCastingTriggered)
            effectSetting.TriggerDestroySpell();

    }

}
