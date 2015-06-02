using UnityEngine;
using System.Collections;

public class RayCastMotor : SpellMotor
{
    [SerializeField]
    private RayCastMotorMethod _motorMethod;
    [SerializeField]
    private float _rayDistance = 5f;
    [SerializeField]
    [Tooltip("This is how fast the ray will go until it reaches the rayDistance")]
    private float _raySpeed = 1f;
    [SerializeField]
    [Tooltip("This is the radius of the sphere cast")]
    private float _sphereRadius = 1f;
    [SerializeField]
    [Tooltip("If this is true it will attempt to trigger a collision with every Entity in the way of the ray. If this is false it will only Entity")]
    private bool triggerAll = false;
    [SerializeField]
    [Tooltip("This is how frequent the ray cast motor will attempt to apply the spell")]
    private float applyFrequency = 1f;

    private float _currentDistance;
    private Timer _applyTimer;

    public RayCastMotorMethod MotorMethod { get { return _motorMethod; } set { _motorMethod = value; } }

    public enum RayCastMotorMethod
    {
        RayCast,
        CapsuleCast,
        SphereCast
    }

    public override void InitializeEffect(EffectSetting effectSetting)
    {
        base.InitializeEffect(effectSetting);
        _applyTimer = new Timer(applyFrequency);
    }

    protected override void UpdateSpell()
    {
        base.UpdateSpell();
        UpdateDistance();
        UpdateRayPosition();

        CheckRay();
    }

    private void UpdateDistance()
    {
        _currentDistance = Mathf.Min(_currentDistance + (_raySpeed * Time.deltaTime), _rayDistance);
    }

    private void CheckRay()
    {
        switch (MotorMethod)
        {
            case RayCastMotorMethod.RayCast:
                DoRayCast();
                break;
            case RayCastMotorMethod.SphereCast:
                DoSphereCast();
                break;
        }
    }

    private void DoRayCast()
    {
        var hits = Physics.RaycastAll(new Ray(transform.position, transform.forward), _currentDistance);
        foreach (var hit in hits)
        {
            TryTriggerCollision(new ColliderEventArgs(), hit.collider);
        }
    }

    private void DoSphereCast()
    {
        var hits = Physics.SphereCastAll(new Ray(transform.position, transform.forward), _sphereRadius, _currentDistance);
        foreach (var hit in hits)
        {
            TryTriggerCollision(new ColliderEventArgs(), hit.collider);
        }
    }

    /// <summary>
    /// Updates the Spell's position to match the spell origin
    /// </summary>
    private void UpdateRayPosition()
    {
        effectSetting.spell.transform.position = effectSetting.spell.SpellStartTransform.position;
        effectSetting.spell.transform.forward = effectSetting.spell.SpellStartTransform.forward;
    }

    protected override void effectSetting_OnSpellCollision(ColliderEventArgs args, Collider obj)
    {
        base.effectSetting_OnSpellCollision(args, obj);
        if (obj.gameObject.layer == LayerMask.NameToLayer("Entity"))
        {
            Entity e = obj.gameObject.GetComponent<Entity>();
            if (e != null)
                effectSetting.TriggerApplySpell(e);
        }
    }

}
