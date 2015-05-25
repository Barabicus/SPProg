using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class SpellEmission : SpellEffectStandard
{

    public EmissionEvent emissionEvent;
    public int emitAmount = 0;

    private ParticleSystem particleSystem;

    #region Start State

    private bool r_emit;
    private bool r_playing;

    #endregion
    public enum EmissionEvent
    {
        DeactivateEmission,
        ActivateEmission,
        EmitAmount,
        Play,
        Stop
    }

    public override void InitializeEffect(EffectSetting effectSetting)
    {
        base.InitializeEffect(effectSetting);
                particleSystem = GetComponent<ParticleSystem>();
        r_playing = particleSystem.playOnAwake;
        r_emit = particleSystem.enableEmission;
    }

    protected override void OnSpellStart()
    {
        base.OnSpellStart();
        if(r_playing)
            particleSystem.Play();
        else
        {
            particleSystem.Stop();
        }

        particleSystem.enableEmission = r_emit;
    }

    protected override void DoEventTriggered()
    {
        base.DoEventTriggered();
        switch (emissionEvent)
        {
            case EmissionEvent.ActivateEmission:
                DoActivateEmission();
                break;
            case EmissionEvent.DeactivateEmission:
                DoDeactivateEmission();
                break;
            case EmissionEvent.EmitAmount:
                DoEmitAmount();
                break;
            case EmissionEvent.Play:
                DoPlay();
                break;
            case EmissionEvent.Stop:
                DoStop();
                break;
        }
    }

    private void DoStop()
    {
        particleSystem.Stop();
    }

    private void DoPlay()
    {
        particleSystem.Play();
    }

    private void DoActivateEmission()
    {
        particleSystem.enableEmission = true;
    }

    private void DoDeactivateEmission()
    {
        particleSystem.enableEmission = false;
    }

    private void DoEmitAmount()
    {
        particleSystem.Emit(emitAmount);
    }

}
