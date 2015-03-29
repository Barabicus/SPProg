using UnityEngine;
using System.Collections;

public class HomingMissileMotor : MissileMotor
{
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
    }

    private void CheckForTargets()
    {
        foreach (Collider c in Physics.OverlapSphere(effectSetting.transform.position, homingRadius))
        {
            if (c.gameObject.layer == LayerMask.NameToLayer("Entity") && c.gameObject != effectSetting.spell.CastingEntity.gameObject)
            {
                Entity e = c.gameObject.GetComponent<Entity>();
                if (e.LivingState != EntityLivingState.Alive)
                    return;
                homingTarget = c.gameObject.transform;
                speed = homingSpeed;
            }
        }
    }
}
