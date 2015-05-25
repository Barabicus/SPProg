using UnityEngine;
using System.Collections;

/// <summary>
/// Tries to spread the spell onto other entities nearby. Note this motor will not trigger a collision.
/// </summary>
public class SpreadMotor : TimedUpdateableSpellMotor
{
    public float radius = 5f;
    protected override void UpdateSpell()
    {
        base.UpdateSpell();
        TrySpread();
    }

    private void TrySpread()
    {
        Collider[] colls = Physics.OverlapSphere(effectSetting.transform.position, radius, 1 << 9);
        foreach (Collider c in colls)
        {
            if (c.gameObject != effectSetting.spell.CastingEntity.gameObject)
            {
                Entity ent = c.gameObject.GetComponent<Entity>();

                if (ent.HasAttachedSpell(effectSetting.spell))
                {
                    Spell sp = SpellList.Instance.GetNewSpell(effectSetting.spell);
                    sp.CastSpell(effectSetting.spell.CastingEntity, ent.transform);
                 //   sp.SetupSpellTransform(ent.transform);
                    ent.AttachSpell(sp);
                }
            }
        }
    }


    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.parent.position, radius);
    }

}
