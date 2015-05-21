using System;
using UnityEngine;
using System.Collections;
using System.Security;

public class BounceSpell : SpellEffect
{
    public float radius = 5f;

    public bool bounceLimit = true;
    public int maxBounceAmount = 1;
    public bool ignoreHitMarker = false;
    public BounceState bounceState = BounceState.UseCastSpell;
    public Spell customSpell;

    private int _currentBounces;

    public enum BounceState
    {
        UseCastSpell,
        UseCustomSpell
    }


    protected override void OnSpellStart()
    {
        base.OnSpellStart();
        _currentBounces = 0;
    }

    protected override void effectSetting_OnSpellApply(Entity entity)
    {
        base.effectSetting_OnSpellApply(entity);
        TryBounce(entity);
    }

    private void TryBounce(Entity hitEnt)
    {

        if (!ignoreHitMarker && hitEnt.HasSpellMarker(SpellMarker) || _currentBounces == maxBounceAmount)
            return;

        Collider[] colls = Physics.OverlapSphere(effectSetting.transform.position, radius, 1 << LayerMask.NameToLayer("Entity"));

        foreach (var c in colls)
        {
            if (c.gameObject != hitEnt.gameObject && c.gameObject != effectSetting.spell.CastingEntity.gameObject)
            {
                Entity targetEnt = c.gameObject.GetComponent<Entity>();

                if (targetEnt == null || targetEnt.LivingState != EntityLivingState.Alive)
                    continue;

                if (SpellMarker != null && targetEnt.HasSpellMarker(SpellMarker))
                    continue;

                if(!targetEnt.IsEnemy(effectSetting.spell.CastingEntity))
                    continue;

                Vector3 startVector = new Vector3(hitEnt.transform.position.x, effectSetting.spell.transform.position.y, hitEnt.transform.position.z);
                Spell sp = null;

                switch (bounceState)
                {
                    case BounceState.UseCastSpell:
                        sp = SpellList.Instance.GetNewSpell(effectSetting.spell);
                        break;
                    case BounceState.UseCustomSpell:
                        sp = SpellList.Instance.GetNewSpell(customSpell);
                        break;
                }
                sp.CastSpell(effectSetting.spell.CastingEntity, null, startVector, targetEnt.transform, targetEnt.transform.position);
               
                //Ignore the entity the spell was bounced from
                sp.IgnoreEntities.Add(hitEnt);

                if (bounceLimit)
                {
                    _currentBounces++;
                    if (_currentBounces == maxBounceAmount)
                        return;
                }

            }
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
