using UnityEngine;
using System.Collections;

public class ShakeCameraOnCollision : SpellEffect
{

    public float shakeAmount = 1f;
    public float maxShakeAmount = 1f;
    public float shakeTime = 0.25f;

    protected override void effectSetting_OnSpellCollision(ColliderEventArgs args, Collider obj)
    {
        base.effectSetting_OnSpellCollision(args, obj);
        GameMainReferences.Instance.RTSCamera.TriggerShake(shakeTime, shakeAmount);
    }

}
