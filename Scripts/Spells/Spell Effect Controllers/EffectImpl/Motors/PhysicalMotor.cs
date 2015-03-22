using UnityEngine;
using System.Collections;

public class PhysicalMotor : SpellEffect
{

    public float range = 2f;

    protected override void UpdateSpell()
    {
        base.UpdateSpell();

        if (Vector3.Distance(effectSetting.spell.SpellTarget.position, effectSetting.spell.CastingEntity.transform.position) <= range)
        {
            effectSetting.TriggerCollision(new ColliderEventArgs(), effectSetting.spell.SpellTarget.GetComponent<Collider>());
            Entity e = effectSetting.spell.SpellTarget.GetComponent<Entity>();
            if (e != null)
                effectSetting.TriggerApplySpell(e);
        }

    }
}
