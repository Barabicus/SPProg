using UnityEngine;
using System.Collections;

public class HomingMissileMotor : MissileMotor
{
    public bool onlyTargetEnemies = true;
    public bool ignoreEntitiesWithSpellMarker = false;
    public float searchTime = 1f;
    public float homingRadius = 10f;
    public float homingSpeed = 30f;

    [Tooltip("The target to move towards. If the target is null the homing missle will attempt to find one throughout its life based on homingradius and homingpspeed")]
    private Transform _homingTarget;
    private Timer _searchTimer;
    private Entity _targetEntity;
    private float r_speed;

    public override Vector3 Direction
    {
        get
        {
            if (_homingTarget == null)
                return base.Direction;
            else
                return (_homingTarget.position - effectSetting.transform.position).normalized + DirectionRandomOffset;
        }
    }

    protected override void OnSpellStart()
    {
        base.OnSpellStart();
        _searchTimer = new Timer(searchTime);
        _targetEntity = null;
        _homingTarget = null;
        r_speed = speed;
        //  CheckForTargets();
    }

    protected override void UpdateSpell()
    {
        base.UpdateSpell();
        if (_homingTarget == null && _searchTimer.CanTickAndReset())
        {
            CheckForTargets();
        }

        if (_homingTarget != null)
        {
            if(Vector3.Distance(effectSetting.transform.position, _homingTarget.position) <= 1.5f)
                TryTriggerCollision(null, _homingTarget.GetComponent<Collider>());

            if (_targetEntity != null && _targetEntity.LivingState != EntityLivingState.Alive)
            {
                _targetEntity = null;
                _homingTarget = null;
                speed = r_speed;
            }
        }
    }

    private void CheckForTargets()
    {
        Transform nearestTransform = null;
        float nearestDistance = 9999999f;
        Entity ent = null;
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
                    ent = e;
                    nearestDistance = distance;
                    nearestTransform = e.transform;
                }


            }
        }

        if (nearestTransform != null)
        {
            _targetEntity = ent;
            _homingTarget = nearestTransform;
            speed = homingSpeed;
        }
    }
}
