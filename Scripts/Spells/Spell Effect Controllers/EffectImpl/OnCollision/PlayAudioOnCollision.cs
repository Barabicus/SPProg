using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class PlayAudioOnCollision : SpellEffect
{
    public AudioClip audioClip;

    private AudioSource _audioSource;

    protected override void OnSpellStart()
    {
        base.OnSpellStart();
        _audioSource = GetComponent<AudioSource>();
    }

    protected override void effectSetting_OnSpellCollision(ColliderEventArgs args, Collider obj)
    {
        base.effectSetting_OnSpellCollision(args, obj);
        _audioSource.PlayOneShot(audioClip);
    }

}
