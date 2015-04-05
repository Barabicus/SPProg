using UnityEngine;
using System.Collections;

public class DummyController : EntityAI
{

    protected override void EntityKilled(Entity e)
    {
        base.EntityKilled(e);
        Invoke("Resurrect", 5f);
    }

    private void Resurrect()
    {
        Entity.CurrentHp = Entity.MaxHp;
        Entity.LivingState = EntityLivingState.Alive;
    }

}
