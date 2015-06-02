using UnityEngine;
using System.Collections;

/// <summary>
/// The spell motor is class that should be used by spell motors to trigger collisions with objects. By attempting to trigger a collision
/// it will work out if it should or shouldn't collider with a specific Entity depending on various factors such as it's living state or it's faction flags.
/// </summary>
public abstract class SpellMotor : SpellEffect, ISpellMotor
{

    [SerializeField]
    [Tooltip("The spell will only be applied to the Entity if the Entity's living state is set to this")]
    private MotorEntityTriggerState _entityTriggerState = MotorEntityTriggerState.Living;
    [SerializeField]
    private bool onlyCollideWithEnemies = true;

    public MotorEntityTriggerState EntityTriggerState
    {
        get { return _entityTriggerState; }
    }

    public void TryTriggerCollision(ColliderEventArgs args, Collider c)
    {
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
