using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class StopEmission : SpellEffect
{
    public StopEmissionTrigger stopEmissionTriggeredBy;
    public float stopEmissionTimeDelay = 0f;
    public bool emitState = false;

    private Timer timer;
    private ParticleSystem particleSystem;

    protected override void Start()
    {
        base.Start();
        particleSystem = GetComponent<ParticleSystem>();
        timer = new Timer(stopEmissionTimeDelay);
    }

    protected override void effectSetting_OnSpellDestroy(object sender, SpellEventargs e)
    {
        base.effectSetting_OnSpellDestroy(sender, e);
        if (stopEmissionTriggeredBy == StopEmissionTrigger.SpellDestroy)
        {
            SetEmission();
        }
    }

    protected override void UpdateSpell()
    {
        base.UpdateSpell();
        if (stopEmissionTriggeredBy == StopEmissionTrigger.Timed && timer.CanTick)
        {
            SetEmission();
        }
    }

    private void SetEmission()
    {
        particleSystem.enableEmission = emitState;
        enabled = false;
    }

    public enum StopEmissionTrigger
    {
        SpellDestroy,
        Timed
    }

}