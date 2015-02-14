using UnityEngine;
using System.Collections;

public class HomingMissileMotor : MissileMotor
{
    public float searchTime = 1f;
    public float radius = 10f;
    public float homingSpeed = 30f;
    public float homingRandomRadius = 0f;

    private Transform _homingTarget;
    private Timer _searchTimer;

    protected override Vector3 Direction
    {
        get
        {
            if (_homingTarget == null)
                return base.Direction;
            else
                return (_homingTarget.position - effectSetting.transform.position).normalized + DirectionRandomOffset;
        }
    }

    protected override void Start()
    {
        base.Start();
        _searchTimer = new Timer(searchTime);
    }

    protected override void UpdateSpell()
    {
        base.UpdateSpell();
        if (_homingTarget == null && _searchTimer.CanTickAndReset())
        {
            foreach (Collider c in Physics.OverlapSphere(effectSetting.transform.position, radius))
            {
                if (c.gameObject.layer == LayerMask.NameToLayer("Entity") && c.gameObject != effectSetting.spell.CastingEntity.gameObject)
                {
                    _homingTarget = c.gameObject.transform;
                    speed = homingSpeed;
                    randomRadius = homingRandomRadius;
                }
            }
        }
    }
}
