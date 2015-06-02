using UnityEngine;
using System.Collections;

public class AttachOnCollision : SpellEffect
{
    public Spell[] attachSpells;

    protected override void effectSetting_OnSpellApply(Entity entity)
    {
        base.effectSetting_OnSpellApply(entity);

        if (entity != null)
        {
            foreach (Spell atch in attachSpells)
            {
                if (atch.SpellType != SpellType.Attached)
                {
                    Debug.LogError("Trying to attach spell: " + atch.SpellID + " which is of type: " + atch.SpellType);
                    continue;
                }
                if (!entity.HasAttachedSpell(atch))
                {
                    Spell sp = SpellList.Instance.GetNewSpell(atch);
                    sp.CastSpell(effectSetting.spell.CastingEntity, entity.transform, null, entity.transform);
                    // sp.SetupSpellTransform(obj.transform);
                    entity.AttachSpell(sp);
                }
                else
                {
                    entity.RefreshAttachedSpell(atch);
                }
            }
        }
    }

}
