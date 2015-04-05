using UnityEngine;
using System.Collections;

/// <summary>
/// Constantly causes the spell to trigger an apply spell update after the specified amount of time has passed
/// </summary>
public class AttachMotor : SpellEffect
{
    public bool singleShot = false;
    public float updateTime = 1f;
    private float lastUpdateTime;

    private Entity targetEntity;

    protected override void Start()
    {
        base.Start();
        targetEntity = effectSetting.transform.parent.GetComponent<Entity>();
        if (targetEntity == null)
        {
            Debug.LogError("Target entity for " + name + " was not an entity. Parent: " + transform.parent.name);
            Destroy(gameObject);
            return;
        }
        lastUpdateTime = Time.time;
    }

    protected override void UpdateSpell()
    {
        base.UpdateSpell();
        if(targetEntity != null && targetEntity.LivingState != EntityLivingState.Alive)
            effectSetting.TriggerDestroy();

        if (targetEntity != null && Time.time - lastUpdateTime >= updateTime)
        {
            effectSetting.TriggerApplySpell(targetEntity);
            lastUpdateTime = Time.time;
            if (singleShot)
                enabled = false;
        }
    }

}
