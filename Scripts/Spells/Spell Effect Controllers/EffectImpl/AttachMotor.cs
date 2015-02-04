﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Constantly causes the spell to trigger an update after the specified amount of time has passed
/// </summary>
public class AttachMotor : SpellEffect
{
    public bool singleShot = false;
    public float updateTime = 2f;
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
        if (targetEntity != null && Time.time - lastUpdateTime >= updateTime)
        {
            effectSetting.spell.ApplySpell(targetEntity);
            lastUpdateTime = Time.time;
            if (singleShot)
                enabled = false;
        }

        if (targetEntity == null)
        {
            Debug.Log(name + " : " + transform.parent.name);
        }
    }

}
