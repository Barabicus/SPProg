using UnityEngine;
using System.Collections;

/// <summary>
/// Constantly causes the spell to trigger an apply spell update after the specified amount of time has passed
/// </summary>
public class AttachMotor : SpellMotor
{
    public bool singleShot = false;
    public float updateTime = 1f;
    private float lastUpdateTime;

    private Entity targetEntity;
    private bool r_enabled;
    private bool _fired;

    public override void InitializeEffect(EffectSetting effectSetting)
    {
        base.InitializeEffect(effectSetting);
        r_enabled = enabled;
    }

    protected override void OnSpellStart()
    {
        base.OnSpellStart();
        enabled = r_enabled;
        _fired = false;
        targetEntity = effectSetting.spell.SpellTarget.GetComponent<Entity>();
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
        if (targetEntity != null && targetEntity.LivingState != EntityLivingState.Alive)
        {
            effectSetting.TriggerDestroySpell();
        }

        if (!_fired)
            DoApply();

    }

    private void DoApply()
    {
        if (targetEntity != null && Time.time - lastUpdateTime >= updateTime)
        {
            effectSetting.TriggerApplySpell(targetEntity);
            lastUpdateTime = Time.time;
            if (singleShot)
                _fired = true;
        }
    }

}
