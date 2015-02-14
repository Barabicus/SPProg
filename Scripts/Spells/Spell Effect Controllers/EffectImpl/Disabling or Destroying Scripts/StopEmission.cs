using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class StopEmission : SpellEffect
{

    ParticleSystem particleSystem;

    protected override void Start()
    {
        base.Start();
        particleSystem = GetComponent<ParticleSystem>();
    }

    protected override void effectSetting_OnSpellDestroy(object sender, SpellEventargs e)
    {
        base.effectSetting_OnSpellDestroy(sender, e);
        particleSystem.enableEmission = false;
    }

}