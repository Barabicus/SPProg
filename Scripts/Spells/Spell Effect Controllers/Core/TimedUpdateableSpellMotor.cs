using UnityEngine;
using System.Collections;

public abstract class TimedUpdateableSpellMotor : TimedUpdateableEffect, ISpellMotor
{
    [SerializeField]
    private MotorEntityTriggerState _entityTriggerState = MotorEntityTriggerState.Living;

    public MotorEntityTriggerState EntityTriggerState
    {
        get { return _entityTriggerState; }
    }

    public void TryTriggerCollision(ColliderEventArgs args, Collider c)
    {
        if (c.gameObject.layer == LayerMask.NameToLayer("Entity"))
        {
            Entity e = c.gameObject.GetComponent<Entity>();
            switch (EntityTriggerState)
            {
                case MotorEntityTriggerState.Dead:
                    if (e.LivingState != EntityLivingState.Dead)
                        return;
                    break;
                case MotorEntityTriggerState.Living:
                    if (e.LivingState != EntityLivingState.Alive)
                        return;
                    break;
            }
        }

        effectSetting.TriggerCollision(new ColliderEventArgs(), c);
    }
}
