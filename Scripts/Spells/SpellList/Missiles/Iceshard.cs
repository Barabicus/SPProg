using UnityEngine;
using System.Collections;

public class Iceshard : MissileSpell
{

    public override float SpellLiveTime
    {
        get { return 20f; }
    }

    public override SpellID SpellID
    {
        get { return global::SpellID.IceShard; }
    }

    public override ElementalStats ElementalPower
    {
        get { return new ElementalStats(0, 5f); }
    }

    public override SpellType SpellType
    {
        get { return global::SpellType.Missile; }
    }

    public override float SpellCastDelay
    {
        get { return 0.75f; }
    }
}
