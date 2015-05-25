using UnityEngine;
using System.Collections;

public class AttachOnCollision : SpellEffect
{
    public Spell[] attachSpells;

    protected override void effectSetting_OnSpellCollision(ColliderEventArgs args, Collider obj)
    {
        base.effectSetting_OnSpellCollision(args, obj);
        Entity ent = obj.GetComponent<Entity>();
        if (ent != null)
        {
            foreach (Spell atch in attachSpells)
            {
                if (atch.SpellType != SpellType.Attached)
                {
                    Debug.LogError("Trying to attach spell: " + atch.SpellID + " which is of type: " + atch.SpellType);
                    continue;
                }
                if (ent.HasAttachedSpell(atch))
                {
                    Spell sp = SpellList.Instance.GetNewSpell(atch);
                    sp.CastSpell(effectSetting.spell.CastingEntity, obj.transform, null, obj.transform);
                   // sp.SetupSpellTransform(obj.transform);
                    ent.AttachSpell(sp);
                }
            }
        }

    }

}
