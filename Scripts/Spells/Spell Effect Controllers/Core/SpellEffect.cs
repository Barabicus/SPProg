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

    private float _currentLivingTime;

    public float CurrentLivingTime
    {
        get { return _currentLivingTime; }
    }

    /// <summary>
    /// A percent from 0-1 on how long this spell has been alive compared to its live time.
    /// </summary>
    public float CurrentLivingTimePercent
    {
        get
        {
            return CurrentLivingTime / effectSetting.spell.SpellLiveTime;
        }
    }

    protected bool OnlyUpdateOnSpellEnabled
    {
        get { return onlyUpdateOnSpellEnabled && !effectSetting.spell.enabled; }
    }

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        effectSetting = transform.parent.GetComponent<EffectSetting>();
        effectSetting.OnSpellDestroy += effectSetting_OnSpellDestroy;
        effectSetting.OnSpellCollision += effectSetting_OnSpellCollision;
        effectSetting.OnEffectDestroy += effectSetting_OnEffectDestroy;
        effectSetting.OnSpellApply += effectSetting_OnSpellApply;

    }

    protected virtual void effectSetting_OnSpellApply(Entity entity)
    {
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

    /// <summary>
    /// This should control update flow and not update logic
    /// </summary>
    protected virtual void Update()
    {
        // If only update on spell enabled is checked, check to see if the spell is enabled
        // if not return
        if (OnlyUpdateOnSpellEnabled)
            return;
        UpdateSpell();

        _currentLivingTime += Time.deltaTime;
    }

    /// <summary>
    /// This should control spell update logic
    /// </summary>
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
