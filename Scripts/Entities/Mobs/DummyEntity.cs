using UnityEngine;
using System.Collections;

public class DummyEntity : HumanoidEntity
{
    
    protected override void Start()
    {
        base.Start();
    }

    protected override void EntityKilled()
    {
        base.EntityKilled();
        CurrentHP = MaxHP;
        Invoke("Resurrect", 5f);
    }

    void Resurrect()
    {
        LivingState = EntityLivingState.Alive;
        MotionState = EntityMotionState.Pathfinding;
    }


    protected override void LivingUpdate()
    {
        base.LivingUpdate();
    }

    public override bool KeepBeamAlive()
    {
        return false;
    }

}
