﻿using UnityEngine;
using System.Collections;

public class SpellResurrect : SpellEffect
{

    protected override void effectSetting_OnSpellApply(Entity entity)
    {
        base.effectSetting_OnSpellApply(entity);
        entity.CurrentHP = entity.MaxHP;
        entity.LivingState = EntityLivingState.Alive;
        entity.MotionState = EntityMotionState.Pathfinding;
    }


}
