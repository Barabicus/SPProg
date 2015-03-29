using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Entity))]
public class EntityAI : MonoBehaviour
{

    public Entity Entity
    {
        get;
        set;
    }

    protected virtual void Start()
    {
        Entity = GetComponent<Entity>();
        Entity.entityKilled += EntityKilled;
    }

    protected virtual void Update()
    {
        switch (Entity.LivingState)
        {
            case EntityLivingState.Alive:
                LivingUpdate();
                break;
            case EntityLivingState.Dead:
                DeadUpdate();
                break;
        }

        switch (Entity.MotionState)
        {
            case EntityMotionState.Pathfinding:
                NavMeshUpdate();
                break;
            case EntityMotionState.Rigidbody:
                RigidBodyUpdate();
                break;
            case EntityMotionState.Static:
                StaticUpdate();
                break;
        }
    }

    /// <summary>
    /// Called while the entity is Living
    /// </summary>
    protected virtual void LivingUpdate() { }
    /// <summary>
    /// Called while the entity is Dead
    /// </summary>
    protected virtual void DeadUpdate() { }

    /// <summary>
    /// Called while the rigid body is controlling entity motion
    /// </summary>
    protected virtual void RigidBodyUpdate() { }
    /// <summary>
    /// Called while the navmesh is controlling entity motion
    /// </summary>
    protected virtual void NavMeshUpdate() { }
    /// <summary>
    /// Called while the entity motion is not being controlled by anything
    /// </summary>
    protected virtual void StaticUpdate() { }

    protected virtual void EntityKilled(Entity e)
    {

    }

}
