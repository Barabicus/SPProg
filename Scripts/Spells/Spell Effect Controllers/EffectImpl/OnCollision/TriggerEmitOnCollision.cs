﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Will cause the emit to emit particles when the collision trigger has been called
/// </summary>
public class TriggerEmitOnCollision : SpellEffect
{

    public int emitAmount = 100;
    public float timeDelay = 0f;
    private ParticleSystem particleSystem;
    private Timer timeDelayTimer;

    // Use this for initialization
    protected override void OnSpellStart()
    {
        base.OnSpellStart();
        particleSystem = GetComponent<ParticleSystem>();
        timeDelayTimer = new Timer(timeDelay);
    }


    protected override void effectSetting_OnSpellCollision(ColliderEventArgs args, Collider obj)
    {
        base.effectSetting_OnSpellCollision(args, obj);
        if (timeDelayTimer.CanTickAndReset())
            particleSystem.Emit(emitAmount);
    }
}