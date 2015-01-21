using UnityEngine;
using System.Collections;

public class FireBall : MissileSpell {

    public override SpellID SpellID
    {
        get { return SpellID.Fireball; }
    }

    public override void Update()
    {
        base.Update();
    }


    public override float SpellLiveTime
    {
        get { return 5f; }
    }

    public override ElementalStats ElementalPower
    {
        get { return new ElementalStats(1f, 0f); }
    }

}
