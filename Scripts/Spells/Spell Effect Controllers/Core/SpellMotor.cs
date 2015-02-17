using UnityEngine;
using System.Collections;

public abstract class SpellMotor : SpellEffect
{

    [Tooltip("Should the motor trigger collision events with entities of the same flag type")]
    public bool collideWithSameFlag = false;

}
