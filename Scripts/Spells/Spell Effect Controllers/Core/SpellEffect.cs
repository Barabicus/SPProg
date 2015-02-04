using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Spell effects modify how the spell behaves. For example the missile motor will control how the spell will move through space
///  while the beam motor will raycast a beam through world space. Spell effects are intended to be drivers for the actual spell
///  and will not execute spell logic.
/// </summary>
public abstract class SpellEffect : MonoBehaviour
{

    public EffectSetting effectSetting;
    public bool onlyUpdateOnSpellEnabled = true;

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        effectSetting = transform.parent.GetComponent<EffectSetting>();
        effectSetting.OnSpellDestroy += effectSetting_OnSpellDestroy;
        effectSetting.OnSpellCollision += effectSetting_OnSpellCollision;
        effectSetting.OnEffectDestroy += effectSetting_OnEffectDestroy;

    }

    protected virtual void effectSetting_OnEffectDestroy()
    {
    }

    protected virtual void effectSetting_OnSpellCollision(ColliderEventArgs args, Collider obj)
    {
    }

    protected virtual void effectSetting_OnSpellDestroy(object sender, SpellEventargs e)
    {
    }

    private void Update()
    {
        if (onlyUpdateOnSpellEnabled && !effectSetting.spell.enabled)
            return;
        UpdateSpell();
    }

    protected virtual void UpdateSpell() { }

}

public class ColliderEventArgs : EventArgs
{
    public Vector3 collPoints;

    public ColliderEventArgs(Vector3 collPoints)
    {
        this.collPoints = collPoints;
    }

    public ColliderEventArgs()
    {
    }

}
