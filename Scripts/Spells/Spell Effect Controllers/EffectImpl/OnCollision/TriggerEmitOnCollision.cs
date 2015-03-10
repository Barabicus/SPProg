using UnityEngine;
using System.Collections;

public class TriggerEmitOnCollision : SpellEffect
{

    public int emitAmount = 100;
    public float timeDelay = 0.2f;
    private ParticleSystem particleSystem;
    private Timer timeDelayTimer;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
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
