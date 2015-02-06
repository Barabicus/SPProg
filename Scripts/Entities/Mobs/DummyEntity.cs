using UnityEngine;
using System.Collections;

public class DummyEntity : Entity
{

    protected override void Start()
    {
        base.Start();
    }

    protected override void EntityKilled()
    {
        base.EntityKilled();
        currentHP = MaxHP;
        Invoke("Resurrect", 5f);
    }

    void Resurrect()
    {
        LivingState = EntityLivingState.Alive;
    }


    protected override void LivingUpdate()
    {
        base.LivingUpdate();
    }

    protected override bool KeepBeamAlive()
    {
        return false;
    }

}
