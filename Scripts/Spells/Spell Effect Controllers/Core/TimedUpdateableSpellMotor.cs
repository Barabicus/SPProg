using UnityEngine;
using System.Collections;

public abstract class TimedUpdateableSpellMotor : TimedUpdateableEffect
{

    [Tooltip("Should the motor trigger collision events with entities of the same flag type")]
    public bool collideWithSameFlag = false;

}
