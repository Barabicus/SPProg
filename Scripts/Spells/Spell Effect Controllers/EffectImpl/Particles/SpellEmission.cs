using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class SpellEmission : SpellEffectStandard
{

    public EmissionEvent emissionEvent;
    public int emitAmount = 0;

    private ParticleSystem particleSystem;

    public enum EmissionEvent
    {
        DeactivateEmission,
        ActivateEmission,
        EmitAmount,
        Play,
        Stop
    }

    protected override void Start()
    {
        base.Start();
        particleSystem = GetComponent<ParticleSystem>();
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
