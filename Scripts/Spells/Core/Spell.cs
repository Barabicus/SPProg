﻿using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(EffectSetting))]
public abstract class Spell : MonoBehaviour
{

    #region Fields

    Rigidbody rigidbody;

    #endregion

    #region Properties
    public Entity CastingEntity { get; set; }

    public Transform SpellTarget
    {
        get;
        set;
    }

    public Vector3? SpellTargetPosition
    {
        get;
        set;
    }

    public Vector3 SpellStartPosition
    {
        get;
        set;
    }

    public Transform SpellStartTransform
    {
        get;
        set;
    }

    public abstract float SpellLiveTime
    {
        get;
    }

    public abstract SpellType SpellType
    {
        get;
    }

    /// <summary>
    /// How much delay this spell leaves before you can cast another spell
    /// </summary>
    public abstract float SpellCastDelay
    {
        get;
    }

    /// <summary>
    /// Creates a transform when the spell is created. This transform will be associated with
    /// SpellTarget when the spell is destroyed this transform is also destroyed.
    /// </summary>
    public virtual bool CreateTransformOnLoad
    {
        get { return false; }
    }

    #endregion

    #region States



    #endregion

    #region Events

    public event EventHandler<SpellEventargs> OnSpellDestroy;

    #endregion 


    public virtual void Awake() { }

    public virtual void Start()
    {
        gameObject.layer = 10;

        Invoke("DestroySpell", SpellLiveTime);
    }

    public virtual bool RequireTarget { get { return false; } }
    public virtual bool RequireTransform { get { return false; } }

    public abstract SpellID SpellID { get; }

    public void CastSpell(Entity castingEntity, Transform startPosition)
    {
        CastingEntity = castingEntity;
        SpellStartTransform = startPosition;
        SpellStartPosition = startPosition.position;
        transform.position = SpellStartPosition;
        transform.rotation = CastingEntity.transform.rotation;
        if (CreateTransformOnLoad)
            SpellTarget = new GameObject("Target: " + gameObject.name).transform;
        gameObject.SetActive(true);
    }


    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void ApplySpell(Entity entity)
    {
        entity.SpellCastBy(new SpellEventargs(this));
    }

    /// <summary>
    /// Called when the spell is destroyed
    /// </summary>
    public virtual void DestroySpell()
    {
        DestroyEvent();
        enabled = false;
        // Destroy the target transform if it was one that was created on load
        if (CreateTransformOnLoad)
            Destroy(SpellTarget.gameObject);
    }


    #region Trigger Events

    public virtual void CollisionEvent(Collider other)
    {
    }

    public virtual void DestroyEvent()
    {
        if (OnSpellDestroy != null)
            OnSpellDestroy(this, new SpellEventargs(this));
    }

    #endregion

}

public enum SpellType
{
    Missile,
    SelfCast,
    Beam,
    Physical
}

public class SpellEventargs : EventArgs
{
    public Spell spell;

    public SpellEventargs(Spell spell)
    {
        this.spell = spell;
    }

}