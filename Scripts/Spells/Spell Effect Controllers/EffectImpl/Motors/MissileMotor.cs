using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class MissileMotor : SpellEffect
{
    public float speed = 2f;
    public float randomRadius = 0f;
    public float randomSpeed = 1f;
    public bool randomXMove = false;
    public bool randomYMove = false;
    public bool randomZMove = false;
    [Tooltip("The speed curve for this spell effect. Y axis is modifer X axis is living time percent from 0 - 1")]
    public AnimationCurve speedCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
    [Tooltip("The Direction offset in spell living time. Note this is spell living real time and not a percent")]
    public AnimationCurve directionXCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);
    [Tooltip("The Direction offset in spell living time. Note this is spell living real time and not a percent")]
    public AnimationCurve directionYCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);
    [Tooltip("The Direction offset in spell living time. Note this is spell living real time and not a percent")]
    public AnimationCurve directionZCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);


    private Vector3 direction;
    private bool shouldMove = true;

    private float _lastTime;
    private float _timeStartOffset;

    private float xRandDir, yRandDir, zRandDir;
    private float xRandSpeed, yRandSpeed, zRandSpeed;


    protected virtual Vector3 Direction
    {
        get
        {
            return direction + DirectionRandomOffset;
        }
    }

    public Vector3 DirectionRandomOffset
    {
        get
        {
            return transform.TransformDirection(new Vector3(xRandDir, yRandDir, zRandDir));
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
        get { return CurrentTime + _timeStartOffset; }
    }

    protected float CurrentTime
    {
        get { return Time.time - _lastTime; }
    }

    protected override void Start()
    {
        base.Start();
        _lastTime = Time.time;
        direction = ((effectSetting.spell.SpellTargetPosition.Value + transform.forward) - effectSetting.transform.position);
        direction.y = 0;
        direction.Normalize();
        GetComponent<Rigidbody>().isKinematic = true;
        transform.parent.forward = direction;
        InitRandomVariables();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != effectSetting.spell.CastingEntity.gameObject && other.gameObject.layer != LayerMask.NameToLayer("Spell") && other.gameObject.layer != LayerMask.NameToLayer("Ground") && other.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
        {
            effectSetting.TriggerCollision(new ColliderEventArgs(), other);
        }
    }

    protected override void UpdateSpell()
    {
        base.UpdateSpell();
        UpdateRandomValues();
        effectSetting.transform.forward = Direction;
        effectSetting.transform.position += DirectionOffset;
        if (shouldMove)
            effectSetting.transform.position += Direction * (speed * speedCurve.Evaluate(CurrentLivingTimePercent)) * Time.deltaTime;
    }

    private void InitRandomVariables()
    {
        _timeStartOffset = Random.Range(0, 100);
        UpdateRandomSpeed();
    }

    private void UpdateRandomSpeed()
    {
        if (randomXMove)
            xRandSpeed = Random.Range(25, 50) * randomSpeed;
        if (randomYMove)
            yRandSpeed = Random.Range(25, 50) * randomSpeed;
        if (randomZMove)
            zRandSpeed = Random.Range(25, 50) * randomSpeed;
    }

    private void UpdateRandomValues()
    {
        xRandDir = (CurrentTimeAndOffset * xRandSpeed) * Mathf.Deg2Rad;
        xRandDir = Mathf.Sin(xRandDir) * randomRadius;

        yRandDir = (CurrentTimeAndOffset * yRandSpeed) * Mathf.Deg2Rad;
        yRandDir = Mathf.Sin(yRandDir) * randomRadius;

        zRandDir = (CurrentTimeAndOffset * zRandSpeed) * Mathf.Deg2Rad;
        zRandDir = Mathf.Sin(zRandDir) * randomRadius;
    }

    protected override void effectSetting_OnSpellDestroy(object sender, SpellEventargs e)
    {
        base.effectSetting_OnSpellDestroy(sender, e);
        shouldMove = false;
    }

}
