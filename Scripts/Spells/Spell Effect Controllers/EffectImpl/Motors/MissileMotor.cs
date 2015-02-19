﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class MissileMotor : SpellEffect
{
    public float speed = 2f;

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

    public AnimationCurve randomXRadiusCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
    public AnimationCurve randomYRadiusCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
    public AnimationCurve randomZRadiusCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

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
        get { return new Vector3(randomXRadius * randomXRadiusCurve.Evaluate(CurrentLivingTimePercent), randomYRadius * randomYRadiusCurve.Evaluate(CurrentLivingTimePercent), randomZRadius * randomZRadiusCurve.Evaluate(CurrentLivingTimePercent)); }
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

    protected override void Start()
    {
        base.Start();
        _lastTime = Time.time;
        direction = ((effectSetting.spell.SpellTargetPosition.Value) - effectSetting.spell.SpellStartPosition);
        direction.y = 0;
        direction.Normalize();
        GetComponent<Rigidbody>().isKinematic = true;
        InitRandomVariables();
        transform.parent.forward = Direction;

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
        if (shouldMove)
        {
            transform.parent.forward = Direction;
            effectSetting.transform.position += DirectionOffset;
            effectSetting.transform.position += (Direction * Speed) + Vector3.Scale(DirectionRandomOffset, RandomRadius);
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

    protected override void effectSetting_OnSpellDestroy(object sender, SpellEventargs e)
    {
        base.effectSetting_OnSpellDestroy(sender, e);
        shouldMove = false;
    }

}