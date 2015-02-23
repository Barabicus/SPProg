using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class StopEmission : SpellEffect
{
    public StopEmissionTrigger stopEmissionTriggeredBy;
    public float stopEmissionTimeDelay = 1f;

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
            _StopEmission();
        }
    }

    protected override void UpdateSpell()
    {
        base.UpdateSpell();
        if (stopEmissionTriggeredBy == StopEmissionTrigger.Timed && timer.CanTick)
        {
            _StopEmission();
        }
    }

    private void _StopEmission()
    {
        particleSystem.enableEmission = false;
        enabled = false;
    }

    public enum StopEmissionTrigger
    {
        SpellDestroy,
        Timed
    }

}