using UnityEngine;
using System.Collections;

public abstract class BeamSpell : MissileSpell {

    public override abstract float SpellLiveTime
    {
        get;
    }

    public override abstract SpellID SpellID
    {
        get;
    }

    public override abstract ElementalStats ElementalPower
    {
        get;
    }

    public override abstract SpellType SpellType
    {
        get;
    }

    public override abstract float SpellCastDelay
    {
        get;
    }

    public override void Update()
    {
        base.Update();
        if (!Input.GetMouseButton(1))
        {
            TriggerDestroyEvent();
        }
    }
}
