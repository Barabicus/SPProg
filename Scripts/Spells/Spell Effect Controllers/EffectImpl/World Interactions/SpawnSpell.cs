using UnityEngine;
using System.Collections;

public class SpawnSpell : SpellEffect
{
    public Spell spawnSpell;

    protected override void Start()
    {
        base.Start();
        Spell sp = SpellList.Instance.GetNewSpell(spawnSpell);
        sp.CastSpell(effectSetting.spell.CastingEntity, transform);
        sp.SpellTarget = effectSetting.spell.SpellTarget;
        sp.SpellTargetPosition = effectSetting.spell.SpellTargetPosition;
        enabled = false;
    }

}
