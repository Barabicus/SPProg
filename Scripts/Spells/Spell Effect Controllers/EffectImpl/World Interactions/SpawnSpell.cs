using UnityEngine;
using System.Collections;

public class SpawnSpell : SpellEffect
{
    public Spell spawnSpell;

    protected override void OnSpellStart()
    {
        base.OnSpellStart();
        Spell sp = SpellList.Instance.GetNewSpell(spawnSpell);
        sp.CastSpell(effectSetting.spell.CastingEntity, transform, transform.position, effectSetting.spell.SpellTarget, effectSetting.spell.SpellTargetPosition);
       // sp.SetupSpellTransform(transform);
        //sp.SpellTarget = effectSetting.spell.SpellTarget;
        //sp.SpellTargetPosition = effectSetting.spell.SpellTargetPosition;
        enabled = false;
    }

}
