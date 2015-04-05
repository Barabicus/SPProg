using UnityEngine;
using System.Collections;

public class HomingMissileMotor : MissileMotor
{
    public bool onlyTargetEnemies = true;
    public bool ignoreEntitiesWithSpellMarker = false;
    public float searchTime = 1f;
    public float homingRadius = 10f;
    public float homingSpeed = 30f;

    [Tooltip("The target to move towards. If the target is null the homing missle will attempt to find one throughout its life based on homingradius and homingpseed")]
    public Transform homingTarget;
    private Timer _searchTimer;

    public override Vector3 Direction
    {
        get
        {
            if (homingTarget == null)
                return base.Direction;
            else
                return (homingTarget.position - effectSetting.transform.position).normalized + DirectionRandomOffset;
        }
    }

    protected override void Start()
    {
        base.Start();
        _searchTimer = new Timer(searchTime);
        //  CheckForTargets();
    }

    protected override void UpdateSpell()
    {
        base.UpdateSpell();
        if (homingTarget == null && _searchTimer.CanTickAndReset())
        {
            CheckForTargets();
        }

        if (homingTarget != null)
        {
            if(Vector3.Distance(effectSetting.transform.position, homingTarget.position) <= 1.5f)
                TryTriggerCollision(null, homingTarget.GetComponent<Collider>());
        }
    }

    private void CheckForTargets()
    {
        Transform nearestTransform = null;
        float nearestDistance = 9999999f;
        foreach (Collider c in Physics.OverlapSphere(effectSetting.transform.position, homingRadius, 1 << LayerMask.NameToLayer("Entity")))
        {
            if (c.gameObject != effectSetting.spell.CastingEntity.gameObject)
            {
                Entity e = c.gameObject.GetComponent<Entity>();
                if (e == null || e.LivingState != EntityLivingState.Alive || (onlyTargetEnemies && !e.IsEnemy(effectSetting.spell.CastingEntity)) || (ignoreEntitiesWithSpellMarker &&  e.HasSpellMarker(SpellMarker)) )
                    continue;

                float distance = Vector3.Distance(e.transform.position, effectSetting.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestTransform = e.transform;
                }


            }
        }

        if (nearestTransform != null)
        {
            homingTarget = nearestTransform;
            speed = homingSpeed;
        }
    }
}
