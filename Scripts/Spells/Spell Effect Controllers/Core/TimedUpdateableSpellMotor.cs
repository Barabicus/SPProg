using UnityEngine;
using System.Collections;

public abstract class TimedUpdateableSpellMotor : TimedUpdateableEffect, ISpellMotor
{
    [SerializeField]
    private MotorEntityTriggerState _entityTriggerState = MotorEntityTriggerState.Living;

    [SerializeField]
    private bool onlyCollideWithEnemies = true;

    public MotorEntityTriggerState EntityTriggerState
    {
        get { return _entityTriggerState; }
    }

    public void TryTriggerCollision(ColliderEventArgs args, Collider c)
    {
        // Don't trigger with a mood box
        if (c.gameObject.layer == LayerMask.NameToLayer("MoodBox"))
            return;

        if (c.gameObject.layer == LayerMask.NameToLayer("Entity"))
        {
            Entity e = c.gameObject.GetComponent<Entity>();

            if (e == null || onlyCollideWithEnemies && !effectSetting.spell.CastingEntity.IsEnemy(e) || effectSetting.spell.IgnoreEntities.Contains(e))
                return;
            
            switch (EntityTriggerState)
            {
                case MotorEntityTriggerState.Dead:
                    if (e.LivingState != EntityLivingState.Dead)
                        return;
                    break;
                case MotorEntityTriggerState.Living:
                    if (e.LivingState != EntityLivingState.Alive)
                        return;
                    break;
            }
        }

        effectSetting.TriggerCollision(new ColliderEventArgs(), c);
    }
}
