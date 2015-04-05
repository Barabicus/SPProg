using UnityEngine;
using System.Collections;

public class SpawnSpellOnCollision : SpellEffect
{
    public Spell spawnSpell;
    protected override void effectSetting_OnSpellCollision(ColliderEventArgs args, Collider obj)
    {
        base.effectSetting_OnSpellCollision(args, obj);
        Spell sp = SpellList.Instance.GetNewSpell(spawnSpell);
        sp.CastSpell(effectSetting.spell.CastingEntity);
        sp.SetupSpellTransform(transform);
        enabled = false;
    }
}
