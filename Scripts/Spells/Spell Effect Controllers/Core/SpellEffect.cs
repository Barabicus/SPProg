using UnityEngine;
using System.Collections;
using System;

public abstract class SpellEffect : MonoBehaviour
{

    public EffectSetting effectSetting;

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        effectSetting = transform.parent.GetComponent<EffectSetting>();
        effectSetting.OnSpellDestroy += effectSetting_OnSpellDestroy;
        effectSetting.OnSpellCollision += effectSetting_OnSpellCollision;

    }

    protected virtual void effectSetting_OnSpellCollision(ColliderEventArgs args, Collider obj)
    {
    }

    protected virtual void effectSetting_OnSpellDestroy(object sender, SpellEventargs e)
    {
    }

    protected virtual void Update() { }

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
