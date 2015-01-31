using UnityEngine;
using System.Collections;

public class PhysicalAttack : ElementalSpell
{

    public override float SpellLiveTime
    {
        get { return 0.5f; }
    }

    //public override SpellID SpellID
    //{
    //    get { return global::SpellID.PhysicalAttack; }
    //}

    public override ElementalStats ElementalPower
    {
        get { return new ElementalStats(0, 0, 1f); }
    }

    public override SpellType SpellType
    {
        get { return global::SpellType.Physical; }
    }

    public override float SpellCastDelay
    {
        get { return 0.5f; }
    }

    public override void CollisionEvent(Collider other)
    {
        base.CollisionEvent(other);
        if (other.gameObject.layer == LayerMask.NameToLayer("Entity"))
            ApplySpell(other.GetComponent<Entity>());
    }
}
