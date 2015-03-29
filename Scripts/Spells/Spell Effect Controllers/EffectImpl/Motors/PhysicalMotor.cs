using UnityEngine;
using System.Collections;

public class PhysicalMotor : SpellMotor
{

    public float range = 2f;

    protected override void UpdateSpell()
    {
        base.UpdateSpell();

        if (Vector3.Distance(effectSetting.spell.SpellTarget.position, effectSetting.spell.CastingEntity.transform.position) <= range)
        {
            TryTriggerCollision(new ColliderEventArgs(), effectSetting.spell.SpellTarget.GetComponent < Collider>());
         //   effectSetting.TriggerCollision(new ColliderEventArgs(), effectSetting.spell.SpellTarget.GetComponent<Collider>());
        }

    }

    protected override void effectSetting_OnSpellCollision(ColliderEventArgs args, Collider obj)
    {
        base.effectSetting_OnSpellCollision(args, obj);
        Entity e = effectSetting.spell.SpellTarget.GetComponent<Entity>();
        if (e != null)
            effectSetting.TriggerApplySpell(e);
    }
}
