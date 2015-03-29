using UnityEngine;
using System.Collections;

public interface ISpellMotor
{
    MotorEntityTriggerState EntityTriggerState
    {
        get;
    }

    void TryTriggerCollision(ColliderEventArgs args, Collider c);
}

public enum MotorEntityTriggerState
{
    Always,
    Living,
    Dead,
}
