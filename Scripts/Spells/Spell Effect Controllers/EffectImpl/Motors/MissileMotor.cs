using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class MissileMotor : SpellMotor
{
    public float speed = 2f;
    public bool keepDistanceToGround = false;
    [Tooltip("If true the missile direction will be within the same y axis as the caster. This does not apply if modified by a direction offset or random offset")]
    public bool negateYDirection = true;
    public float minGroundDistance = 1f;

    [Tooltip("The speed curve for this spell effect. Y axis is modifer X axis is living time percent from 0 - 1")]
    public AnimationCurve speedCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
    [Tooltip("The Direction offset in spell living time. Note this is spell living real time and not a percent")]
    public AnimationCurve directionXCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);
    [Tooltip("The Direction offset in spell living time. Note this is spell living real time and not a percent")]
    public AnimationCurve directionYCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);
    [Tooltip("The Direction offset in spell living time. Note this is spell living real time and not a percent")]
    public AnimationCurve directionZCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);

    public bool randomXMove = false;
    public bool randomYMove = false;
    public bool randomZMove = false;
    public float randomXRadius = 1f;
    public float randomYRadius = 1f;
    public float randomZRadius = 1f;

    [Tooltip("The Random x radius multiplier curve based over spell life time")]
    public AnimationCurve randomXRadiusCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
    [Tooltip("The Random y radius multiplier curve based over spell life time")]
    public AnimationCurve randomYRadiusCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
    [Tooltip("The Random z radius multiplier curve based over spell life time")]
    public AnimationCurve randomZRadiusCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

    [Tooltip("The random amount of sin cycles that should occur. A Higher number will lead to rapid cycles leading to a lower radius area")]
    public float minRange = 1f;
    public float randomRange = 1f;




    private Vector3 direction;
    private bool shouldMove = true;

    private float _lastTime;
    private float _timeStartOffset;

    private float xRandDir = 0f, yRandDir = 0f, zRandDir = 0f;
    private float xRandSpeed, yRandSpeed, zRandSpeed;


    public virtual Vector3 Direction
    {
        get
        {
            return (direction);
        }
    }

    public Vector3 RandomRadius
    {
        get
        {
            return transform.TransformDirection(new Vector3(randomXRadius * randomXRadiusCurve.Evaluate(CurrentLivingTimePercent), randomYRadius * randomYRadiusCurve.Evaluate(CurrentLivingTimePercent), randomZRadius * randomZRadiusCurve.Evaluate(CurrentLivingTimePercent)));
        }
    }

    protected virtual float Speed
    {
        get
        {
            return (speed * speedCurve.Evaluate(CurrentLivingTimePercent)) * Time.deltaTime;
        }
    }

    public Vector3 DirectionRandomOffset
    {
        get
        {
            return transform.TransformDirection(new Vector3(xRandDir, yRandDir, zRandDir)) * Time.deltaTime;
        }
    }

    public Vector3 DirectionOffset
    {
        get
        {
            return transform.TransformDirection(new Vector3(directionXCurve.Evaluate(CurrentTime), directionYCurve.Evaluate(CurrentTime), directionZCurve.Evaluate(CurrentTime))) * Time.deltaTime;
        }
    }

    protected float CurrentTimeAndOffset
    {
        get { return (CurrentTime) + _timeStartOffset; }
    }

    protected float CurrentTime
    {
        get { return Time.time - _lastTime; }
    }

    public override void InitializeEffect(EffectSetting effectSetting)
    {
        base.InitializeEffect(effectSetting);
        GetComponent<Rigidbody>().isKinematic = true;
        Collider[] colliders = GetComponents<Collider>();
        if (colliders.Length == 0)
            Debug.LogError("Spell: " + effectSetting.spell.spellName + " has no colliders");

        // Ensure all colliders are set to trigger
        foreach (Collider c in colliders)
        {
            c.isTrigger = true;
        }
    }

    protected override void OnSpellStart()
    {
        base.OnSpellStart();
        shouldMove = true;
        _lastTime = Time.time;
        direction = ((effectSetting.spell.SpellTargetPosition.Value) - effectSetting.spell.SpellStartPosition);
        if (negateYDirection)
            direction.y = 0;
        direction.Normalize();

        InitRandomVariables();
        transform.parent.forward = Direction;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != effectSetting.spell.CastingEntity.gameObject && other.gameObject.layer != LayerMask.NameToLayer("Spell") && other.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
        {
            TryTriggerCollision(new ColliderEventArgs(), other);
        }
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

    protected override void UpdateSpell()
    {
        base.UpdateSpell();
        UpdateRandomValues();
        effectSetting.transform.forward = Direction;
        if (shouldMove)
        {
            transform.parent.forward = Direction;
            effectSetting.transform.position += DirectionOffset;
            effectSetting.transform.position += (Direction * Speed) + Vector3.Scale(DirectionRandomOffset, RandomRadius);

            if (keepDistanceToGround)
            {
                RaycastHit hit;
                if (Physics.Raycast(effectSetting.transform.position, -Vector3.up, out hit, 500f, 1 << LayerMask.NameToLayer("Ground")))
                {
                    if (Mathf.Abs(hit.point.y - transform.position.y) < minGroundDistance)
                        effectSetting.transform.position = new Vector3(effectSetting.transform.position.x, hit.point.y + minGroundDistance, effectSetting.transform.position.z);
                }
            }
        }

    }

    private void InitRandomVariables()
    {
        _timeStartOffset = Random.Range(0, 1000);

        UpdateRandomSpeed();
        UpdateRandomValues();
    }

    private void UpdateRandomSpeed()
    {
        if (randomXMove)
            xRandSpeed = (Random.Range(minRange * 1000, 1000 * randomRange) + 1) / 1000f;
        if (randomYMove)
            yRandSpeed = (Random.Range(minRange * 1000, 1000 * randomRange) + 1) / 1000f;
        if (randomZMove)
            zRandSpeed = (Random.Range(minRange * 1000, 1000 * randomRange) + 1) / 1000f;
    }

    private void UpdateRandomValues()
    {
        if (randomXMove)
        {
            xRandDir = (CurrentTimeAndOffset * xRandSpeed) * Mathf.Deg2Rad;
            xRandDir = Mathf.Sin(xRandDir);
        }

        if (randomYMove)
        {
            yRandDir = (CurrentTimeAndOffset * yRandSpeed) * Mathf.Deg2Rad;
            yRandDir = Mathf.Sin(yRandDir);
        }

        if (randomZMove)
        {
            zRandDir = (CurrentTimeAndOffset * zRandSpeed) * Mathf.Deg2Rad;
            zRandDir = Mathf.Sin(zRandDir);
        }
    }

    protected override void effectSetting_OnSpellDestroy()
    {
        base.effectSetting_OnSpellDestroy();
        shouldMove = false;
    }

}
